using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;

namespace CleanInk.OnCall.Application.Encounters.Queries;

/// <summary>Returns all encounters for a given patient, optionally filtered by status.</summary>
/// <param name="PatientId">Patient identifier.</param>
/// <param name="Status">Optional status filter (InProgress, Finished, Cancelled).</param>
public sealed record GetEncountersByPatientQuery(Guid PatientId, string? Status) : IRequest<Result<IReadOnlyList<EncounterDto>>>;

/// <summary>Handler for <see cref="GetEncountersByPatientQuery"/>.</summary>
public sealed class GetEncountersByPatientQueryHandler
    : IRequestHandler<GetEncountersByPatientQuery, Result<IReadOnlyList<EncounterDto>>>
{
    private readonly IEncounterRepository _encounters;

    /// <summary>Initializes a new instance of <see cref="GetEncountersByPatientQueryHandler"/>.</summary>
    public GetEncountersByPatientQueryHandler(IEncounterRepository encounters) => _encounters = encounters;

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<EncounterDto>>> Handle(
        GetEncountersByPatientQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.EncounterStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<Domain.Entities.EncounterStatus>(request.Status, ignoreCase: true, out var parsed))
        {
            statusFilter = parsed;
        }

        var items = await _encounters.GetByPatientAsync(request.PatientId, statusFilter, cancellationToken);
        IReadOnlyList<EncounterDto> dtos = items.Select(EncounterDto.FromEntity).ToList();
        return Result<IReadOnlyList<EncounterDto>>.Success(dtos);
    }
}
