using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Billing aggregate root — FHIR R4 Invoice.
///
/// Domain invariants:
/// 1. <see cref="AmountCents"/> must be strictly positive.
/// 2. A "Paid" invoice CANNOT be cancelled — only a credit note (avoir) can be issued.
/// 3. A "Cancelled" invoice is IMMUTABLE.
/// 4. <see cref="Reference"/> is unique per tenant (DB constraint + domain).
/// </summary>
public sealed class Invoice : Entity<Guid>
{
    private Invoice() { }

    public Guid PatientId { get; private set; }
    public Guid? EncounterId { get; private set; }
    public Guid? PractitionerId { get; private set; }
    public string Reference { get; private set; } = string.Empty;

    /// <summary>Amount before VAT in euro cents. Must be &gt; 0.</summary>
    public int AmountCents { get; private set; }

    /// <summary>VAT amount in euro cents.</summary>
    public int VatCents { get; private set; }

    public int TotalCents => AmountCents + VatCents;
    public decimal AmountEuros => AmountCents / 100m;

    public InvoiceStatus Status { get; private set; }

    public Guid? ThirdPartyPayerClaimId { get; private set; }
    public string? PaymentMethod { get; private set; }

    public DateTime IssuedAt { get; private set; }
    public DateTime DueAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Factory — creates a new draft invoice.
    /// Raises <see cref="InvoiceCreatedEvent"/>.
    /// </summary>
    public static Result<Invoice> Create(
        Guid patientId,
        string reference,
        int amountCents,
        int vatCents = 0,
        Guid? encounterId = null,
        Guid? practitionerId = null,
        DateTime? dueAt = null)
    {
        if (patientId == Guid.Empty)
            return Error.Validation(nameof(PatientId), "Patient ID is required.");

        if (string.IsNullOrWhiteSpace(reference))
            return Error.Validation(nameof(Reference), "Invoice reference is required.");

        if (amountCents <= 0)
            return Error.Validation(nameof(AmountCents), "Invoice amount must be strictly positive.");

        if (vatCents < 0)
            return Error.Validation(nameof(VatCents), "VAT amount cannot be negative.");

        var now = DateTime.UtcNow;
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            EncounterId = encounterId,
            PractitionerId = practitionerId,
            Reference = reference.Trim(),
            AmountCents = amountCents,
            VatCents = vatCents,
            Status = InvoiceStatus.Draft,
            IssuedAt = now,
            DueAt = dueAt ?? now.AddDays(30),
            UpdatedAt = now,
        };

        invoice.RaiseDomainEvent(new InvoiceCreatedEvent(invoice.Id, patientId, invoice.TotalCents, now));
        return invoice;
    }

    /// <summary>Finalizes the invoice (Draft → Issued).</summary>
    public Result Issue()
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("Invoice.NotDraft", "Only a draft invoice can be issued."));

        Status = InvoiceStatus.Issued;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>Records payment. Invariant: paid invoice cannot be cancelled.</summary>
    public Result MarkPaid(string paymentMethod)
    {
        if (Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled)
            return Result.Failure(new Error("Invoice.Immutable",
                $"Cannot mark an invoice in '{Status}' state as paid."));

        if (string.IsNullOrWhiteSpace(paymentMethod))
            return Result.Failure(Error.Validation(nameof(paymentMethod), "Payment method is required."));

        Status = InvoiceStatus.Paid;
        PaymentMethod = paymentMethod.Trim();
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new InvoicePaidEvent(Id, PatientId, AmountCents, paymentMethod, DateTime.UtcNow));
        return Result.Success;
    }

    /// <summary>
    /// Cancels the invoice.
    /// Invariant: PAID invoice cannot be cancelled — issue a credit note (avoir) instead.
    /// </summary>
    public Result Cancel(string reason)
    {
        if (Status == InvoiceStatus.Paid)
            return Result.Failure(new Error("Invoice.PaidCannotCancel",
                "A paid invoice cannot be cancelled. Issue a credit note (avoir) instead."));

        if (Status == InvoiceStatus.Cancelled)
            return Result.Failure(new Error("Invoice.AlreadyCancelled", "This invoice is already cancelled."));

        Status = InvoiceStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    public void LinkThirdPartyPayerClaim(Guid claimId)
    {
        ThirdPartyPayerClaimId = claimId;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum InvoiceStatus
{
    Draft = 0,
    Issued = 1,
    Paid = 2,
    Cancelled = 3,
    Disputed = 4,
}

