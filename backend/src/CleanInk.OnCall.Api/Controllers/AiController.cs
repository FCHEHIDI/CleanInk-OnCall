using CleanInk.OnCall.Infrastructure.AI.Agents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Exposes AI agent endpoints: triage, summary, and compliance checks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AiController : ControllerBase
{
    private readonly AgentOrchestrator _orchestrator;

    /// <summary>
    /// Initializes a new instance of <see cref="AiController"/>.
    /// </summary>
    /// <param name="orchestrator">The agent orchestrator.</param>
    public AiController(AgentOrchestrator orchestrator) => _orchestrator = orchestrator;

    /// <summary>
    /// Classifies the provided text into a triage tag.
    /// </summary>
    /// <param name="request">Input text payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the triage tag string.</response>
    /// <response code="400">Empty input.</response>
    [HttpPost("triage")]
    [ProducesResponseType(typeof(AiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Triage([FromBody] AiRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var result = await _orchestrator.RunAsync("triage", request.Text, ct);
        return Ok(new AiResponse(result));
    }

    /// <summary>
    /// Generates a concise summary of the provided text.
    /// </summary>
    /// <param name="request">Input text payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the summary string.</response>
    /// <response code="400">Empty input.</response>
    [HttpPost("summary")]
    [ProducesResponseType(typeof(AiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Summary([FromBody] AiRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var result = await _orchestrator.RunAsync("summary", request.Text, ct);
        return Ok(new AiResponse(result));
    }

    /// <summary>
    /// Runs a GDPR/compliance audit on the provided text.
    /// </summary>
    /// <param name="request">Input text payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the compliance report.</response>
    /// <response code="400">Empty input.</response>
    [HttpPost("compliance")]
    [ProducesResponseType(typeof(AiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Compliance([FromBody] AiRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var result = await _orchestrator.RunAsync("compliance", request.Text, ct);
        return Ok(new AiResponse(result));
    }
}

/// <summary>Request payload for AI endpoints.</summary>
/// <param name="Text">The text to process.</param>
public record AiRequest(string Text);

/// <summary>Response payload from AI endpoints.</summary>
/// <param name="Result">The AI-generated text output.</param>
public record AiResponse(string Result);
