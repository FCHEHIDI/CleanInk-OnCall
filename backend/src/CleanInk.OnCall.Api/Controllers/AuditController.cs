using CleanInk.OnCall.Application.Auth;
using CleanInk.OnCall.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Audit log controller — Admin only.
/// Provides HDS-compliant access to the audit trail for CNIL investigations
/// and internal compliance reviews.
/// </summary>
[ApiController]
[Route("api/audit")]
[Authorize(Policy = "AuditAccess")]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditRepository _audit;

    /// <summary>Initializes a new instance of <see cref="AuditController"/>.</summary>
    public AuditController(IAuditRepository audit) => _audit = audit;

    /// <summary>
    /// Returns all audit entries for a specific entity.
    /// </summary>
    /// <param name="entityType">Entity type: "Call", "Patient", "User", "Invoice".</param>
    /// <param name="entityId">Entity ID (string).</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("{entityType}/{entityId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntity(
        string entityType, string entityId, CancellationToken ct)
    {
        var entries = await _audit.GetByEntityAsync(entityType, entityId, ct);
        return Ok(entries.Select(e => new
        {
            e.Id,
            e.ActorEmail,
            e.Action,
            e.EntityType,
            e.EntityId,
            e.OldValues,
            e.NewValues,
            e.IpAddress,
            e.OccurredAt
        }));
    }

    /// <summary>
    /// Returns all audit entries performed by a specific actor.
    /// </summary>
    /// <param name="actorId">Actor user ID.</param>
    /// <param name="from">Optional UTC start date.</param>
    /// <param name="to">Optional UTC end date.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("actor/{actorId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByActor(
        Guid actorId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var entries = await _audit.GetByActorAsync(actorId, from, to, ct);
        return Ok(entries.Select(e => new
        {
            e.Id,
            e.ActorEmail,
            e.Action,
            e.EntityType,
            e.EntityId,
            e.OccurredAt
        }));
    }
}
