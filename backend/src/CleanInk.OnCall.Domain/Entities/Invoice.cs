using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Represents the payment status of an invoice.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>Invoice has been issued but not yet paid.</summary>
    Pending = 0,

    /// <summary>Invoice has been fully paid.</summary>
    Paid = 1,

    /// <summary>Invoice is overdue.</summary>
    Overdue = 2,

    /// <summary>Invoice has been cancelled.</summary>
    Cancelled = 3
}

/// <summary>
/// Aggregate root representing a billing invoice in the CleanInk OnCall system.
/// Amounts are stored in the smallest currency unit (e.g. euro cents).
/// </summary>
public sealed class Invoice : Entity<Guid>
{
    private Invoice() { }

    /// <summary>Gets the ID of the customer this invoice is for.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>Gets the optional ID of the call this invoice is linked to.</summary>
    public Guid? CallId { get; private set; }

    /// <summary>Gets the invoice reference number (human-readable, e.g. "INV-2024-0042").</summary>
    public string Reference { get; private set; } = string.Empty;

    /// <summary>Gets the amount in euro cents (e.g. 5000 = €50.00).</summary>
    public long AmountCents { get; private set; }

    /// <summary>Gets the three-letter ISO 4217 currency code (e.g. "EUR").</summary>
    public string Currency { get; private set; } = "EUR";

    /// <summary>Gets the current payment status.</summary>
    public InvoiceStatus Status { get; private set; }

    /// <summary>Gets the UTC due date of this invoice.</summary>
    public DateTime DueDate { get; private set; }

    /// <summary>Gets the UTC timestamp when the invoice was created.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Gets the UTC timestamp of the last update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Factory method to create a new <see cref="Invoice"/>.
    /// </summary>
    /// <param name="customerId">ID of the customer.</param>
    /// <param name="reference">Unique human-readable reference.</param>
    /// <param name="amountCents">Amount in smallest currency unit (must be > 0).</param>
    /// <param name="dueDate">UTC due date (must be in the future).</param>
    /// <param name="callId">Optional linked call ID.</param>
    /// <param name="currency">ISO 4217 currency code, defaults to "EUR".</param>
    /// <returns>A <see cref="Result{Invoice}"/> with the new invoice or validation errors.</returns>
    public static Result<Invoice> Create(
        Guid customerId,
        string reference,
        long amountCents,
        DateTime dueDate,
        Guid? callId = null,
        string currency = "EUR")
    {
        if (customerId == Guid.Empty)
            return Error.Validation(nameof(CustomerId), "CustomerId cannot be empty.");

        if (string.IsNullOrWhiteSpace(reference))
            return Error.Validation(nameof(Reference), "Reference is required.");

        if (amountCents <= 0)
            return Error.Validation(nameof(AmountCents), "Amount must be greater than zero.");

        if (dueDate <= DateTime.UtcNow)
            return Error.Validation(nameof(DueDate), "Due date must be in the future.");

        var now = DateTime.UtcNow;
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CallId = callId,
            Reference = reference.Trim(),
            AmountCents = amountCents,
            Currency = currency.ToUpperInvariant(),
            Status = InvoiceStatus.Pending,
            DueDate = dueDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        return invoice;
    }

    /// <summary>Marks the invoice as paid.</summary>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result MarkAsPaid()
    {
        if (Status != InvoiceStatus.Pending)
            return Result.Failure(new Error("Invoice.InvalidTransition",
                $"Cannot mark invoice as paid from status '{Status}'."));

        Status = InvoiceStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>Marks the invoice as overdue.</summary>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result MarkAsOverdue()
    {
        if (Status != InvoiceStatus.Pending)
            return Result.Failure(new Error("Invoice.InvalidTransition",
                $"Cannot mark invoice as overdue from status '{Status}'."));

        Status = InvoiceStatus.Overdue;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>Cancels the invoice.</summary>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            return Result.Failure(new Error("Invoice.InvalidTransition",
                "Cannot cancel a paid invoice."));

        Status = InvoiceStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }
}
