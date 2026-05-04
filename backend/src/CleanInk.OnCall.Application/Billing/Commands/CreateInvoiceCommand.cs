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
/// <param name="PatientId">ID of the patient.</param>
/// <param name="Reference">Unique human-readable reference.</param>
/// <param name="AmountCents">Amount in euro cents (must be positive).</param>
/// <param name="VatCents">VAT amount in euro cents (default 0).</param>
/// <param name="EncounterId">Optional linked encounter ID.</param>
/// <param name="PractitionerId">Optional linked practitioner ID.</param>
public record CreateInvoiceCommand(
    Guid PatientId,
    string Reference,
    int AmountCents,
    int VatCents = 0,
    Guid? EncounterId = null,
    Guid? PractitionerId = null) : IRequest<Result<InvoiceDto>>;

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
            "Creating invoice ref='{Reference}' for patient {PatientId}",
            request.Reference, request.PatientId);

        var invoiceResult = Invoice.Create(
            request.PatientId,
            request.Reference,
            request.AmountCents,
            request.VatCents,
            request.EncounterId,
            request.PractitionerId);

        if (invoiceResult.IsFailure)
        {
            _logger.LogWarning("Invoice creation failed: {Error}", invoiceResult.Error.Description);
            return Result<InvoiceDto>.Failure(invoiceResult.Error);
        }

        var invoice = invoiceResult.Value;
        await _invoices.AddAsync(invoice, cancellationToken);

        _logger.LogInformation("Invoice {InvoiceId} created successfully", invoice.Id);

        return Result<InvoiceDto>.Success(new InvoiceDto(
            invoice.Id,
            invoice.PatientId,
            invoice.EncounterId,
            invoice.Reference,
            invoice.AmountCents,
            invoice.VatCents,
            invoice.Status.ToString(),
            invoice.IssuedAt,
            invoice.DueAt));
    }
}
