using CleanInk.OnCall.Application.Billing.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Billing.Queries;

/// <summary>
/// Query to retrieve a paginated list of invoices, optionally filtered by customer.
/// </summary>
/// <param name="Page">Page number (1-based).</param>
/// <param name="PageSize">Items per page (max 100).</param>
/// <param name="CustomerId">Optional customer filter.</param>
public record GetInvoicesQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? CustomerId = null) : IRequest<Result<PagedResult<InvoiceDto>>>;

/// <summary>
/// Handles <see cref="GetInvoicesQuery"/>.
/// </summary>
public sealed class GetInvoicesQueryHandler
    : IRequestHandler<GetInvoicesQuery, Result<PagedResult<InvoiceDto>>>
{
    private readonly IInvoiceRepository _invoices;
    private readonly ILogger<GetInvoicesQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GetInvoicesQueryHandler"/>.
    /// </summary>
    /// <param name="invoices">Invoice repository.</param>
    /// <param name="logger">Logger instance.</param>
    public GetInvoicesQueryHandler(
        IInvoiceRepository invoices,
        ILogger<GetInvoicesQueryHandler> logger)
    {
        _invoices = invoices;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<InvoiceDto>>> Handle(
        GetInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Fetching invoices page={Page} size={PageSize} customer={CustomerId}",
            request.Page, request.PageSize, request.CustomerId);

        var items = await _invoices.GetByPatientAsync(
            request.CustomerId ?? Guid.Empty,
            ct: cancellationToken);

        var dtos = items.Select(i => new InvoiceDto(
            i.Id,
            i.PatientId,
            i.EncounterId,
            i.Reference,
            i.AmountCents,
            i.VatCents,
            i.Status.ToString(),
            i.IssuedAt,
            i.DueAt)).ToList();

        return Result<PagedResult<InvoiceDto>>.Success(
            new PagedResult<InvoiceDto>(dtos, request.Page, Math.Min(request.PageSize, 100), dtos.Count));
    }
}
