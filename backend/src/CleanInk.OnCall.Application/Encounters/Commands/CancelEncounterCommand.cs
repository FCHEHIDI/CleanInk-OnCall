using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Encounters.Commands;

/// <summary>
/// Cancels an in-progress encounter. A finished encounter cannot be cancelled.
/// </summary>
/// <param name="EncounterId">Encounter to cancel.</param>
/// <param name="Reason">Free-text reason for cancellation.</param>
public sealed record CancelEncounterCommand(Guid EncounterId, string Reason) : IRequest<Result<EncounterDto>>;

/// <summary>Handler for <see cref="CancelEncounterCommand"/>.</summary>
public sealed class CancelEncounterCommandHandler : IRequestHandler<CancelEncounterCommand, Result<EncounterDto>>
{
    private readonly IEncounterRepository _encounters;
    private readonly ILogger<CancelEncounterCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="CancelEncounterCommandHandler"/>.</summary>
    public CancelEncounterCommandHandler(
        IEncounterRepository encounters,
        ILogger<CancelEncounterCommandHandler> logger)
    {
        _encounters = encounters;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<EncounterDto>> Handle(CancelEncounterCommand request, CancellationToken cancellationToken)
    {
        var encounter = await _encounters.GetByIdAsync(request.EncounterId, cancellationToken);
        if (encounter is null)
            return Result<EncounterDto>.Failure(
                Error.NotFound("Encounter.NotFound", $"Encounter {request.EncounterId} not found."));

        var cancelResult = encounter.Cancel(request.Reason);
        if (cancelResult.IsFailure) return Result<EncounterDto>.Failure(cancelResult.Error);

        _encounters.Update(encounter);
        await _encounters.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Encounter {EncounterId} cancelled. Reason: {Reason}",
            request.EncounterId, request.Reason);

        return Result<EncounterDto>.Success(EncounterDto.FromEntity(encounter));
    }
}
