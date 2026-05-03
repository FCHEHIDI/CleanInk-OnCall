using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.AI.AzureOpenAI;

/// <summary>
/// <see cref="IAiService"/> implementation backed by Azure OpenAI.
/// </summary>
public sealed class AzureOpenAiService : IAiService
{
    private readonly AzureOpenAiClient _client;
    private readonly ILogger<AzureOpenAiService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureOpenAiService"/>.
    /// </summary>
    /// <param name="client">The low-level Azure OpenAI HTTP client.</param>
    /// <param name="logger">Logger instance.</param>
    public AzureOpenAiService(AzureOpenAiClient client, ILogger<AzureOpenAiService> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        _logger.LogInformation("AzureOpenAI GenerateAsync called");
        return await _client.SendAsync(prompt, ct);
    }

    /// <inheritdoc/>
    public async Task<string> ChatAsync(string message, CancellationToken ct = default)
    {
        _logger.LogInformation("AzureOpenAI ChatAsync called");
        return await _client.SendAsync(message, ct);
    }
}
