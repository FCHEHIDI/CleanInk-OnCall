using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.AI.Claude;

/// <summary>
/// <see cref="IAiService"/> implementation backed by the Anthropic Claude API.
/// </summary>
public sealed class ClaudeAiService : IAiService
{
    private readonly ClaudeClient _client;
    private readonly ILogger<ClaudeAiService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ClaudeAiService"/>.
    /// </summary>
    /// <param name="client">The low-level Claude HTTP client.</param>
    /// <param name="logger">Logger instance.</param>
    public ClaudeAiService(ClaudeClient client, ILogger<ClaudeAiService> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        _logger.LogInformation("Claude GenerateAsync called");
        return await _client.SendAsync(prompt, ct);
    }

    /// <inheritdoc/>
    public async Task<string> ChatAsync(string message, CancellationToken ct = default)
    {
        _logger.LogInformation("Claude ChatAsync called");
        return await _client.SendAsync(message, ct);
    }
}
