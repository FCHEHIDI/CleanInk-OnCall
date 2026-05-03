using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.AI.Agents;

/// <summary>
/// Agent that generates a concise summary of a call or conversation text.
/// </summary>
public sealed class SummaryAgent : IAgent
{
    private readonly IAiService _ai;
    private readonly ILogger<SummaryAgent> _logger;

    /// <inheritdoc/>
    public string Name => "summary";

    /// <summary>
    /// Initializes a new instance of <see cref="SummaryAgent"/>.
    /// </summary>
    /// <param name="ai">AI service used for summarization.</param>
    /// <param name="logger">Logger instance.</param>
    public SummaryAgent(IAiService ai, ILogger<SummaryAgent> logger)
    {
        _ai = ai;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> RunAsync(string input, CancellationToken ct = default)
    {
        _logger.LogInformation("SummaryAgent processing input length={Length}", input.Length);

        var prompt = $"""
            You are a professional call center summarizer.
            Write a concise summary (3-5 sentences) of the following customer interaction.
            Focus on: the customer's main issue, actions taken, and outcome or next steps.

            Interaction:
            {input}
            """;

        var summary = await _ai.GenerateAsync(prompt, ct);
        _logger.LogInformation("SummaryAgent generated summary length={Length}", summary.Length);
        return summary.Trim();
    }
}
