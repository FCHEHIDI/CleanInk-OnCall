namespace CleanInk.OnCall.Application.Billing.DTOs;

/// <summary>
/// Data transfer object representing an invoice returned by the API.
/// </summary>
/// <param name="Id">Unique identifier of the invoice.</param>
/// <param name="PatientId">ID of the patient.</param>
/// <param name="EncounterId">Linked encounter ID, or null.</param>
/// <param name="Reference">Human-readable invoice reference (e.g. "INV-2024-0042").</param>
/// <param name="AmountCents">Amount in euro cents.</param>
/// <param name="VatCents">VAT amount in euro cents.</param>
/// <param name="Status">Payment status string (e.g. "Draft", "Issued", "Paid").</param>
/// <param name="IssuedAt">UTC issued timestamp.</param>
/// <param name="DueAt">UTC due date.</param>
public record InvoiceDto(
    Guid Id,
    Guid PatientId,
    Guid? EncounterId,
    string Reference,
    int AmountCents,
    int VatCents,
    string Status,
    DateTime IssuedAt,
    DateTime DueAt);
