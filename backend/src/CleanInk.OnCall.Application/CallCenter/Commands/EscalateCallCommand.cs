using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.CallCenter.Commands;

/// <summary>
/// Command to escalate a call to a senior clinician or doctor.
/// Escalation is a critical healthcare workflow step — it triggers notifications
/// and is immutably recorded in the audit trail.
/// </summary>
/// <param name="CallId">ID of the call to escalate.</param>
/// <param name="EscalatedBy">ID of the operator triggering escalation.</param>
/// <param name="Reason">Mandatory clinical reason for escalation (min 10 chars).</param>
public sealed record EscalateCallCommand(Guid CallId, Guid EscalatedBy, string Reason)
    : IRequest<Result>;

/// <summary>Handler for <see cref="EscalateCallCommand"/>.</summary>
public sealed class EscalateCallCommandHandler : IRequestHandler<EscalateCallCommand, Result>
{
    private readonly ICallRepository _calls;
    private readonly ILogger<EscalateCallCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="EscalateCallCommandHandler"/>.</summary>
    public EscalateCallCommandHandler(
        ICallRepository calls,
        ILogger<EscalateCallCommandHandler> logger)
    {
        _calls = calls;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(EscalateCallCommand request, CancellationToken cancellationToken)
    {
        var call = await _calls.GetByIdAsync(request.CallId, cancellationToken);
        if (call is null)
            return Result.Failure(Error.NotFound("Call.NotFound", $"Call {request.CallId} not found."));

        var result = call.Escalate(request.EscalatedBy, request.Reason);
        if (!result.IsSuccess) return result;

        _calls.Update(call);
        await _calls.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Call {CallId} escalated by operator {OperatorId}. Reason: {Reason}",
            request.CallId, request.EscalatedBy, request.Reason);

        return Result.Success;
    }
}
