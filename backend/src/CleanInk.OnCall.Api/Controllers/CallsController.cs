using CleanInk.OnCall.Application.CallCenter.Commands;
using CleanInk.OnCall.Application.CallCenter.DTOs;
using CleanInk.OnCall.Application.CallCenter.Queries;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Manages customer support calls: create, list, assign, resolve, and escalate.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CallsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="CallsController"/>.
    /// </summary>
    /// <param name="mediator">MediatR dispatcher.</param>
    public CallsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of calls.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 20, max 100).</param>
    /// <param name="customerId">Optional customer filter.</param>
    /// <param name="operatorId">Optional operator filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns paginated call list.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CallDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? operatorId = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetCallsQuery(page, pageSize, customerId, operatorId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Creates a new inbound call.
    /// </summary>
    /// <param name="command">Call creation payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="201">Returns the created call.</response>
    /// <response code="400">Validation error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CallDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCallCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (result.IsFailure) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetAll), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>Assigns a call to an operator.</summary>
    [HttpPatch("{id:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignCallRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new AssignCallCommand(id, request.OperatorId), ct);
        if (result.IsSuccess) return NoContent();
        return result.Error.Code.StartsWith("Call.NotFound") ? NotFound() : BadRequest(result.Error);
    }

    /// <summary>Escalates a call to a senior clinician.</summary>
    [HttpPatch("{id:guid}/escalate")]
    [Authorize(Policy = "ClinicalAccess")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Escalate(Guid id, [FromBody] EscalateCallRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new EscalateCallCommand(id, request.EscalatedBy, request.Reason), ct);
        if (result.IsSuccess) return NoContent();
        return result.Error.Code.StartsWith("Call.NotFound") ? NotFound() : BadRequest(result.Error);
    }

    /// <summary>Closes a call with a mandatory clinical summary.</summary>
    [HttpPatch("{id:guid}/close")]
    [Authorize(Policy = "ClinicalAccess")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Close(Guid id, [FromBody] CloseCallRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CloseCallCommand(id, request.ClosedBy, request.Summary), ct);
        if (result.IsSuccess) return NoContent();
        return result.Error.Code.StartsWith("Call.NotFound") ? NotFound() : BadRequest(result.Error);
    }
}

/// <summary>Request body for assigning a call.</summary>
public sealed record AssignCallRequest(Guid OperatorId);

/// <summary>Request body for escalating a call.</summary>
public sealed record EscalateCallRequest(Guid EscalatedBy, string Reason);

/// <summary>Request body for closing a call.</summary>
public sealed record CloseCallRequest(Guid ClosedBy, string Summary);
