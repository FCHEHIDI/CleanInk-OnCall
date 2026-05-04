using CleanInk.OnCall.Application.CallCenter.DTOs;
using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.CallCenter.Commands;

/// <summary>
/// Command to create a new inbound call.
/// </summary>
/// <param name="CreatedByUserId">ID of the user creating the call.</param>
/// <param name="Subject">Short subject line (max 200 chars).</param>
/// <param name="Description">Full call description.</param>
public record CreateCallCommand(
    Guid CreatedByUserId,
    string Subject,
    string Description) : IRequest<Result<CallDto>>;

/// <summary>
/// Handles <see cref="CreateCallCommand"/> — creates and persists a new call aggregate.
/// </summary>
public sealed class CreateCallCommandHandler : IRequestHandler<CreateCallCommand, Result<CallDto>>
{
    private readonly ICallRepository _calls;
    private readonly ILogger<CreateCallCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateCallCommandHandler"/>.
    /// </summary>
    /// <param name="calls">Call repository.</param>
    /// <param name="logger">Logger instance.</param>
    public CreateCallCommandHandler(ICallRepository calls, ILogger<CreateCallCommandHandler> logger)
    {
        _calls = calls;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<CallDto>> Handle(
        CreateCallCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating call by user {CreatedByUserId} with subject '{Subject}'",
            request.CreatedByUserId, request.Subject);

        var callResult = Call.Create(request.CreatedByUserId, request.Subject, request.Description);
        if (callResult.IsFailure)
        {
            _logger.LogWarning("Call creation validation failed: {Error}", callResult.Error.Description);
            return Result<CallDto>.Failure(callResult.Error);
        }

        var call = callResult.Value;
        await _calls.AddAsync(call, cancellationToken);

        _logger.LogInformation("Call {CallId} created successfully", call.Id);

        return Result<CallDto>.Success(new CallDto(
            call.Id,
            call.CreatedByUserId,
            call.AssignedPractitionerId,
            call.Subject,
            call.Description,
            call.Status.ToString(),
            call.AiTriageTag,
            call.AiSummary,
            call.CreatedAt,
            call.UpdatedAt));
    }
}
