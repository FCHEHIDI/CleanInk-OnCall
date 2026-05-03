using CleanInk.OnCall.Application.Billing.DTOs;
using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Billing.Commands;

/// <summary>
/// Command to create a new invoice.
/// </summary>
/// <param name="CustomerId">ID of the customer.</param>
/// <param name="Reference">Unique human-readable reference.</param>
/// <param name="AmountCents">Amount in smallest currency unit (e.g. euro cents).</param>
/// <param name="DueDate">UTC due date (must be in the future).</param>
/// <param name="CallId">Optional linked call ID.</param>
/// <param name="Currency">ISO 4217 currency code (default "EUR").</param>
public record CreateInvoiceCommand(
    Guid CustomerId,
    string Reference,
    long AmountCents,
    DateTime DueDate,
    Guid? CallId = null,
    string Currency = "EUR") : IRequest<Result<InvoiceDto>>;

/// <summary>
/// Handles <see cref="CreateInvoiceCommand"/>.
/// </summary>
public sealed class CreateInvoiceCommandHandler
    : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoices;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateInvoiceCommandHandler"/>.
    /// </summary>
    /// <param name="invoices">Invoice repository.</param>
    /// <param name="logger">Logger instance.</param>
    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoices,
        ILogger<CreateInvoiceCommandHandler> logger)
    {
        _invoices = invoices;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<InvoiceDto>> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating invoice ref='{Reference}' for customer {CustomerId}",
            request.Reference, request.CustomerId);

        var invoiceResult = Invoice.Create(
            request.CustomerId,
            request.Reference,
            request.AmountCents,
            request.DueDate,
            request.CallId,
            request.Currency);

        if (invoiceResult.IsFailure)
        {
            _logger.LogWarning("Invoice creation failed: {Error}", invoiceResult.Error.Description);
            return Result<InvoiceDto>.Failure(invoiceResult.Error);
        }

        var invoice = invoiceResult.Value;
        await _invoices.AddAsync(invoice, cancellationToken);
        await _invoices.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invoice {InvoiceId} created successfully", invoice.Id);

        return Result<InvoiceDto>.Success(new InvoiceDto(
            invoice.Id,
            invoice.CustomerId,
            invoice.CallId,
            invoice.Reference,
            invoice.AmountCents,
            invoice.Currency,
            invoice.Status.ToString(),
            invoice.DueDate,
            invoice.CreatedAt));
    }
}
