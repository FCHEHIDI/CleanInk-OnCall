using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.AI.Agents;

/// <summary>
/// Agent that classifies an inbound call into a triage category.
/// Returns a short tag such as "billing", "technical", "complaint", etc.
/// </summary>
public sealed class TriageAgent : IAgent
{
    private readonly IAiService _ai;
    private readonly ILogger<TriageAgent> _logger;

    /// <inheritdoc/>
    public string Name => "triage";

    /// <summary>
    /// Initializes a new instance of <see cref="TriageAgent"/>.
    /// </summary>
    /// <param name="ai">AI service used for classification.</param>
    /// <param name="logger">Logger instance.</param>
    public TriageAgent(IAiService ai, ILogger<TriageAgent> logger)
    {
        _ai = ai;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> RunAsync(string input, CancellationToken ct = default)
    {
        _logger.LogInformation("TriageAgent processing input length={Length}", input.Length);

        var prompt = $"""
            You are a triage classifier for a customer support call center.
            Classify the following customer message into exactly ONE short lowercase tag.
            Possible tags: billing, technical, complaint, general, escalation, other.
            Respond with the tag only — no punctuation, no explanation.

            Customer message:
            {input}
            """;

        var tag = (await _ai.GenerateAsync(prompt, ct)).Trim().ToLowerInvariant();
        _logger.LogInformation("TriageAgent classified as '{Tag}'", tag);
        return tag;
    }
}
