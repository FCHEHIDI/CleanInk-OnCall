using CleanInk.OnCall.Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Provides aggregated metrics for the operational dashboard.
/// All endpoints require an authenticated JWT and are tenant-scoped.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="DashboardController"/>.
    /// </summary>
    /// <param name="mediator">MediatR dispatcher.</param>
    public DashboardController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Returns live KPI counts: calls today, open calls, pending invoices, active users.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">KPI snapshot for the current tenant.</response>
    /// <response code="401">Missing or invalid JWT.</response>
    [HttpGet("kpis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetKpis(CancellationToken ct = default)
    {
        var kpis = await _mediator.Send(new GetDashboardKpisQuery(), ct);
        return Ok(kpis);
    }
}
