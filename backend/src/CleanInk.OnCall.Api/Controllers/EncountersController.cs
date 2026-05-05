using CleanInk.OnCall.Application.Encounters.Commands;
using CleanInk.OnCall.Application.Encounters.DTOs;
using CleanInk.OnCall.Application.Encounters.Queries;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Manages clinical encounters (FHIR R4 Encounter).
/// Access restricted to clinical staff (Médecin, IDE) or Admin.
///
/// FHIR: https://www.hl7.org/fhir/encounter.html
/// </summary>
[ApiController]
[Route("api/encounters")]
[Authorize(Policy = "ClinicalAccess")]
public sealed class EncountersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="EncountersController"/>.</summary>
    public EncountersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Returns a paginated list of all encounters across all patients.
    /// </summary>
    /// <param name="page">1-based page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 20, max 100).</param>
    /// <param name="status">Optional status filter: InProgress | Finished | Cancelled.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EncounterDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetAllEncountersQuery(status, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Returns all encounters for a patient, ordered by most recent first.
    /// </summary>
    /// <param name="patientId">Patient identifier.</param>
    /// <param name="status">Optional status filter: InProgress | Finished | Cancelled.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("patient/{patientId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<EncounterDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPatient(
        Guid patientId,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetEncountersByPatientQuery(patientId, status), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Returns a single encounter by its identifier.
    /// </summary>
    /// <param name="id">Encounter identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEncounterByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Starts a new clinical encounter.
    /// </summary>
    /// <param name="request">Start encounter payload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Start([FromBody] StartEncounterRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new StartEncounterCommand(
                request.PatientId,
                request.PractitionerId,
                request.EncounterClass,
                request.ReasonText), ct);

        if (result.IsFailure) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Finishes an encounter with a mandatory clinical note.
    /// Once finished, the encounter is immutable.
    /// </summary>
    /// <param name="id">Encounter identifier.</param>
    /// <param name="request">Closing note payload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost("{id:guid}/finish")]
    [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Finish(Guid id, [FromBody] FinishEncounterRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new FinishEncounterCommand(id, request.ClinicalNote), ct);
        if (result.IsFailure)
            return result.Error.Code.Contains("NotFound") ? NotFound(result.Error) : BadRequest(result.Error);
        return Ok(result.Value);
    }

    /// <summary>
    /// Cancels an in-progress encounter.
    /// </summary>
    /// <param name="id">Encounter identifier.</param>
    /// <param name="request">Cancellation reason payload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(EncounterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelEncounterRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelEncounterCommand(id, request.Reason), ct);
        if (result.IsFailure)
            return result.Error.Code.Contains("NotFound") ? NotFound(result.Error) : BadRequest(result.Error);
        return Ok(result.Value);
    }
}

/// <summary>Request body for starting an encounter.</summary>
public sealed record StartEncounterRequest(
    Guid PatientId,
    Guid PractitionerId,
    string EncounterClass,
    string? ReasonText);

/// <summary>Request body for finishing an encounter.</summary>
public sealed record FinishEncounterRequest(string ClinicalNote);

/// <summary>Request body for cancelling an encounter.</summary>
public sealed record CancelEncounterRequest(string Reason);
