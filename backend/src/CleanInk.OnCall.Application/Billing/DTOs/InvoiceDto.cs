namespace CleanInk.OnCall.Application.Billing.DTOs;

/// <summary>
/// Data transfer object representing an invoice returned by the API.
/// </summary>
/// <param name="Id">Unique identifier of the invoice.</param>
/// <param name="CustomerId">ID of the customer.</param>
/// <param name="CallId">Linked call ID, or null.</param>
/// <param name="Reference">Human-readable invoice reference (e.g. "INV-2024-0042").</param>
/// <param name="AmountCents">Amount in smallest currency unit.</param>
/// <param name="Currency">ISO 4217 currency code.</param>
/// <param name="Status">Payment status string (e.g. "Pending", "Paid").</param>
/// <param name="DueDate">UTC due date.</param>
/// <param name="CreatedAt">UTC creation timestamp.</param>
public record InvoiceDto(
    Guid Id,
    Guid CustomerId,
    Guid? CallId,
    string Reference,
    long AmountCents,
    string Currency,
    string Status,
    DateTime DueDate,
    DateTime CreatedAt);
