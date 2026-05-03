using CleanInk.OnCall.Application.Patients.Commands;
using CleanInk.OnCall.Application.Patients.DTOs;
using CleanInk.OnCall.Application.Patients.Queries;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Manages patient records.
/// Access requires clinical role (Médecin, IDE) or Admin.
///
/// RGPD / HDS: All responses mask PII (NIR, phone, email) by default.
/// Raw data is only accessible via explicit elevated request (future sprint).
/// </summary>
[ApiController]
[Route("api/patients")]
[Authorize(Policy = "ClinicalAccess")]
public sealed class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="PatientsController"/>.</summary>
    public PatientsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Searches patients by name fragment with pagination.
    /// Only non-archived patients are returned.
    /// </summary>
    /// <param name="q">Name fragment search (case-insensitive). Empty returns all.</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (max 50).</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PatientDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string q = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPatientsQuery(q, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Retrieves a single patient by ID.
    /// Returns masked PII.
    /// </summary>
    /// <param name="id">Patient identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPatientByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Registers a new patient.
    /// Only LastName, FirstName, and DateOfBirth are required.
    /// PII (NIR, phone, email) is optional — may be added after consent.
    /// </summary>
    /// <param name="request">Patient registration payload.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterPatientRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RegisterPatientCommand(
                request.LastName,
                request.FirstName,
                request.DateOfBirth,
                request.Nir,
                request.Phone,
                request.Email), ct);

        if (result.IsFailure) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }
}

/// <summary>Request body for patient registration.</summary>
/// <param name="LastName">Last name (required).</param>
/// <param name="FirstName">First name (required).</param>
/// <param name="DateOfBirth">Date of birth (required, ISO 8601).</param>
/// <param name="Nir">Optional NIR.</param>
/// <param name="Phone">Optional phone (E.164 or French format).</param>
/// <param name="Email">Optional email address.</param>
public sealed record RegisterPatientRequest(
    string LastName,
    string FirstName,
    DateOnly DateOfBirth,
    string? Nir,
    string? Phone,
    string? Email);
