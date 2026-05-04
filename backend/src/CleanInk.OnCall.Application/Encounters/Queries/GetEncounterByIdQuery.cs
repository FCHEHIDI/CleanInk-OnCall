using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;

namespace CleanInk.OnCall.Application.Encounters.Queries;

/// <summary>Returns a single encounter by its identifier.</summary>
/// <param name="EncounterId">Encounter GUID.</param>
public sealed record GetEncounterByIdQuery(Guid EncounterId) : IRequest<Result<EncounterDto>>;

/// <summary>Handler for <see cref="GetEncounterByIdQuery"/>.</summary>
public sealed class GetEncounterByIdQueryHandler : IRequestHandler<GetEncounterByIdQuery, Result<EncounterDto>>
{
    private readonly IEncounterRepository _encounters;

    /// <summary>Initializes a new instance of <see cref="GetEncounterByIdQueryHandler"/>.</summary>
    public GetEncounterByIdQueryHandler(IEncounterRepository encounters) => _encounters = encounters;

    /// <inheritdoc/>
    public async Task<Result<EncounterDto>> Handle(GetEncounterByIdQuery request, CancellationToken cancellationToken)
    {
        var encounter = await _encounters.GetByIdAsync(request.EncounterId, cancellationToken);
        if (encounter is null)
            return Result<EncounterDto>.Failure(
                Error.NotFound("Encounter.NotFound", $"Encounter {request.EncounterId} not found."));

        return Result<EncounterDto>.Success(EncounterDto.FromEntity(encounter));
    }
}
