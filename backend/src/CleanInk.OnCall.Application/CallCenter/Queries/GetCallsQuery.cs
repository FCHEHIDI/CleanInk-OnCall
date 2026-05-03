using CleanInk.OnCall.Application.CallCenter.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.CallCenter.Queries;

/// <summary>
/// Query to retrieve a paginated list of calls.
/// </summary>
/// <param name="Page">Page number (1-based, default 1).</param>
/// <param name="PageSize">Items per page (default 20, max 100).</param>
/// <param name="CustomerId">Optional customer filter.</param>
/// <param name="OperatorId">Optional operator filter.</param>
public record GetCallsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? CustomerId = null,
    Guid? OperatorId = null) : IRequest<Result<PagedResult<CallDto>>>;

/// <summary>
/// Handles <see cref="GetCallsQuery"/> — fetches paginated call records.
/// </summary>
public sealed class GetCallsQueryHandler : IRequestHandler<GetCallsQuery, Result<PagedResult<CallDto>>>
{
    private readonly ICallRepository _calls;
    private readonly ILogger<GetCallsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GetCallsQueryHandler"/>.
    /// </summary>
    /// <param name="calls">Call repository.</param>
    /// <param name="logger">Logger instance.</param>
    public GetCallsQueryHandler(ICallRepository calls, ILogger<GetCallsQueryHandler> logger)
    {
        _calls = calls;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<CallDto>>> Handle(
        GetCallsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Fetching calls page={Page} size={PageSize} customer={CustomerId} operator={OperatorId}",
            request.Page, request.PageSize, request.CustomerId, request.OperatorId);

        var page = await _calls.GetPagedAsync(
            request.Page,
            Math.Min(request.PageSize, 100),
            request.CustomerId,
            request.OperatorId,
            cancellationToken);

        var dtos = page.Items.Select(c => new CallDto(
            c.Id,
            c.CustomerId,
            c.OperatorId,
            c.Subject,
            c.Description,
            c.Status.ToString(),
            c.AiTriageTag,
            c.AiSummary,
            c.CreatedAt,
            c.UpdatedAt)).ToList();

        return Result<PagedResult<CallDto>>.Success(
            new PagedResult<CallDto>(dtos, page.Page, page.PageSize, page.TotalCount));
    }
}
