using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Encounters.Commands;

/// <summary>
/// Starts a new clinical encounter (patient-practitioner interaction).
/// </summary>
/// <param name="PatientId">Subject patient identifier.</param>
/// <param name="PractitionerId">Responsible practitioner identifier.</param>
/// <param name="EncounterClass">FHIR class: AMB | EMER | IMP | HH.</param>
/// <param name="ReasonText">Optional free-text reason for the encounter.</param>
public sealed record StartEncounterCommand(
    Guid PatientId,
    Guid PractitionerId,
    string EncounterClass,
    string? ReasonText) : IRequest<Result<EncounterDto>>;

/// <summary>Handler for <see cref="StartEncounterCommand"/>.</summary>
public sealed class StartEncounterCommandHandler : IRequestHandler<StartEncounterCommand, Result<EncounterDto>>
{
    private readonly IEncounterRepository _encounters;
    private readonly ILogger<StartEncounterCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="StartEncounterCommandHandler"/>.</summary>
    public StartEncounterCommandHandler(
        IEncounterRepository encounters,
        ILogger<StartEncounterCommandHandler> logger)
    {
        _encounters = encounters;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<EncounterDto>> Handle(StartEncounterCommand request, CancellationToken cancellationToken)
    {
        var result = Encounter.Start(
            request.PatientId,
            request.PractitionerId,
            request.EncounterClass,
            request.ReasonText);

        if (result.IsFailure) return Result<EncounterDto>.Failure(result.Error);

        await _encounters.AddAsync(result.Value, cancellationToken);
        await _encounters.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Encounter {EncounterId} started for patient {PatientId}",
            result.Value.Id, request.PatientId);

        return Result<EncounterDto>.Success(EncounterDto.FromEntity(result.Value));
    }
}
