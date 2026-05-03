using CleanInk.OnCall.Application.Billing.Commands;
using CleanInk.OnCall.Application.Billing.DTOs;
using CleanInk.OnCall.Application.Billing.Queries;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Manages customer invoices: create and list.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="InvoicesController"/>.
    /// </summary>
    /// <param name="mediator">MediatR dispatcher.</param>
    public InvoicesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of invoices.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 20, max 100).</param>
    /// <param name="customerId">Optional customer filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns paginated invoice list.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? customerId = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetInvoicesQuery(page, pageSize, customerId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Creates a new invoice.
    /// </summary>
    /// <param name="command">Invoice creation payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="201">Returns the created invoice.</response>
    /// <response code="400">Validation error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (result.IsFailure) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetAll), new { id = result.Value.Id }, result.Value);
    }
}
