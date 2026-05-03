using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.CallCenter.Commands;

/// <summary>
/// Command to close a call with a mandatory clinical summary.
///
/// Healthcare context:
/// - A summary is MANDATORY for regulatory traceability.
/// - Closing a call triggers invoice creation (via domain event) in a full implementation.
/// - The AI summary agent can pre-populate this if the call was assigned to an AI agent.
/// </summary>
/// <param name="CallId">ID of the call to close.</param>
/// <param name="ClosedBy">ID of the operator closing the call.</param>
/// <param name="Summary">
/// Clinical summary of the call outcome (min 10 chars).
/// This becomes the permanent medical note for this interaction.
/// </param>
public sealed record CloseCallCommand(Guid CallId, Guid ClosedBy, string Summary)
    : IRequest<Result>;

/// <summary>Handler for <see cref="CloseCallCommand"/>.</summary>
public sealed class CloseCallCommandHandler : IRequestHandler<CloseCallCommand, Result>
{
    private readonly ICallRepository _calls;
    private readonly ILogger<CloseCallCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="CloseCallCommandHandler"/>.</summary>
    public CloseCallCommandHandler(
        ICallRepository calls,
        ILogger<CloseCallCommandHandler> logger)
    {
        _calls = calls;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(CloseCallCommand request, CancellationToken cancellationToken)
    {
        var call = await _calls.GetByIdAsync(request.CallId, cancellationToken);
        if (call is null)
            return Result.Failure(Error.NotFound("Call.NotFound", $"Call {request.CallId} not found."));

        var result = call.Close(request.ClosedBy, request.Summary);
        if (!result.IsSuccess) return result;

        _calls.Update(call);
        await _calls.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Call {CallId} closed by {ClosedBy}.", request.CallId, request.ClosedBy);
        return Result.Success;
    }
}
