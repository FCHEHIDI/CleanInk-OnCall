using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;

namespace CleanInk.OnCall.Application.Encounters.Queries;

/// <summary>Returns a paginated list of all encounters across patients, optionally filtered by status.</summary>
/// <param name="Status">Optional status filter (InProgress, Finished, Cancelled).</param>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Items per page (max 100).</param>
public sealed record GetAllEncountersQuery(string? Status, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<EncounterDto>>>;

/// <summary>Handler for <see cref="GetAllEncountersQuery"/>.</summary>
public sealed class GetAllEncountersQueryHandler
    : IRequestHandler<GetAllEncountersQuery, Result<PagedResult<EncounterDto>>>
{
    private readonly IEncounterRepository _encounters;

    /// <summary>Initializes a new instance of <see cref="GetAllEncountersQueryHandler"/>.</summary>
    public GetAllEncountersQueryHandler(IEncounterRepository encounters) => _encounters = encounters;

    /// <inheritdoc/>
    public async Task<Result<PagedResult<EncounterDto>>> Handle(
        GetAllEncountersQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.EncounterStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<Domain.Entities.EncounterStatus>(request.Status, ignoreCase: true, out var parsed))
        {
            statusFilter = parsed;
        }

        var items = await _encounters.GetAllAsync(
            statusFilter,
            request.Page,
            Math.Min(request.PageSize, 100),
            cancellationToken);

        var dtos = items.Select(EncounterDto.FromEntity).ToList();
        return Result<PagedResult<EncounterDto>>.Success(
            new PagedResult<EncounterDto>(dtos, request.Page, Math.Min(request.PageSize, 100), dtos.Count));
    }
}
