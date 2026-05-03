using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.CallCenter.Commands;

/// <summary>Command to assign a pending call to an operator.</summary>
/// <param name="CallId">ID of the call to assign.</param>
/// <param name="OperatorId">ID of the operator taking charge.</param>
public sealed record AssignCallCommand(Guid CallId, Guid OperatorId)
    : IRequest<Result>;

/// <summary>Handler for <see cref="AssignCallCommand"/>.</summary>
public sealed class AssignCallCommandHandler : IRequestHandler<AssignCallCommand, Result>
{
    private readonly ICallRepository _calls;
    private readonly ILogger<AssignCallCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="AssignCallCommandHandler"/>.</summary>
    public AssignCallCommandHandler(
        ICallRepository calls,
        ILogger<AssignCallCommandHandler> logger)
    {
        _calls = calls;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(AssignCallCommand request, CancellationToken cancellationToken)
    {
        var call = await _calls.GetByIdAsync(request.CallId, cancellationToken);
        if (call is null)
            return Result.Failure(Error.NotFound("Call.NotFound", $"Call {request.CallId} not found."));

        var result = call.Assign(request.OperatorId);
        if (!result.IsSuccess) return result;

        _calls.Update(call);
        await _calls.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Call {CallId} assigned to operator {OperatorId}.",
            request.CallId, request.OperatorId);
        return Result.Success;
    }
}
