using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.AI.Agents;

/// <summary>
/// Agent that checks a call or text for GDPR / compliance issues and flags sensitive data.
/// </summary>
public sealed class ComplianceAgent : IAgent
{
    private readonly IAiService _ai;
    private readonly ILogger<ComplianceAgent> _logger;

    /// <inheritdoc/>
    public string Name => "compliance";

    /// <summary>
    /// Initializes a new instance of <see cref="ComplianceAgent"/>.
    /// </summary>
    /// <param name="ai">AI service used for compliance analysis.</param>
    /// <param name="logger">Logger instance.</param>
    public ComplianceAgent(IAiService ai, ILogger<ComplianceAgent> logger)
    {
        _ai = ai;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> RunAsync(string input, CancellationToken ct = default)
    {
        _logger.LogInformation("ComplianceAgent processing input length={Length}", input.Length);

        var prompt = $"""
            You are a GDPR and data-privacy compliance auditor for a customer support call center.
            Analyze the following text and report:
            1. Any personal data (PII) present (names, emails, phone numbers, addresses, payment data).
            2. Potential GDPR risks or violations.
            3. Recommended actions to mitigate risks.
            Be concise. If no issues are found, respond with "No compliance issues detected."

            Text to audit:
            {input}
            """;

        var report = await _ai.GenerateAsync(prompt, ct);
        _logger.LogInformation("ComplianceAgent completed report length={Length}", report.Length);
        return report.Trim();
    }
}
