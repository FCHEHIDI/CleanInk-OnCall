using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Encounters.Commands;

/// <summary>
/// Finishes an encounter by recording the mandatory closing clinical note.
/// A finished encounter is immutable — only addendum notes may be appended.
/// </summary>
/// <param name="EncounterId">Encounter to close.</param>
/// <param name="ClinicalNote">Mandatory closing note from the practitioner.</param>
public sealed record FinishEncounterCommand(Guid EncounterId, string ClinicalNote) : IRequest<Result<EncounterDto>>;

/// <summary>Handler for <see cref="FinishEncounterCommand"/>.</summary>
public sealed class FinishEncounterCommandHandler : IRequestHandler<FinishEncounterCommand, Result<EncounterDto>>
{
    private readonly IEncounterRepository _encounters;
    private readonly ILogger<FinishEncounterCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="FinishEncounterCommandHandler"/>.</summary>
    public FinishEncounterCommandHandler(
        IEncounterRepository encounters,
        ILogger<FinishEncounterCommandHandler> logger)
    {
        _encounters = encounters;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<EncounterDto>> Handle(FinishEncounterCommand request, CancellationToken cancellationToken)
    {
        var encounter = await _encounters.GetByIdAsync(request.EncounterId, cancellationToken);
        if (encounter is null)
            return Result<EncounterDto>.Failure(
                Error.NotFound("Encounter.NotFound", $"Encounter {request.EncounterId} not found."));

        var finishResult = encounter.Finish(request.ClinicalNote);
        if (finishResult.IsFailure) return Result<EncounterDto>.Failure(finishResult.Error);

        _encounters.Update(encounter);
        await _encounters.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Encounter {EncounterId} finished", request.EncounterId);

        return Result<EncounterDto>.Success(EncounterDto.FromEntity(encounter));
    }
}
