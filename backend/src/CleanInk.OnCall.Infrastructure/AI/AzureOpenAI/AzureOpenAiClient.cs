using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanInk.OnCall.Infrastructure.AI.AzureOpenAI;

/// <summary>
/// Low-level HTTP client for the Azure OpenAI Chat Completions API.
/// Calls POST /openai/deployments/{deploymentName}/chat/completions?api-version={version}.
/// </summary>
public sealed class AzureOpenAiClient
{
    private readonly HttpClient _http;
    private readonly AzureOpenAiSettings _settings;
    private readonly ILogger<AzureOpenAiClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Initializes a new instance of <see cref="AzureOpenAiClient"/>.
    /// </summary>
    /// <param name="http">Named HttpClient configured with the Azure OpenAI base address.</param>
    /// <param name="settings">Azure OpenAI configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public AzureOpenAiClient(
        HttpClient http,
        IOptions<AzureOpenAiSettings> settings,
        ILogger<AzureOpenAiClient> logger)
    {
        _http = http;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends a prompt as a user chat message and returns the model's text reply.
    /// </summary>
    /// <param name="prompt">The text to send as a user message.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The model's text response.</returns>
    /// <exception cref="HttpRequestException">Thrown on non-success HTTP status.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response is missing expected fields.</exception>
    public async Task<string> SendAsync(string prompt, CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Sending prompt to Azure OpenAI deployment={Deployment} length={Length}",
            _settings.DeploymentName, prompt.Length);

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = _settings.MaxTokens
        };

        var url = $"/openai/deployments/{_settings.DeploymentName}/chat/completions?api-version={_settings.ApiVersion}";
        using var response = await _http.PostAsJsonAsync(url, requestBody, JsonOptions, ct);
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

        var text = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? throw new InvalidOperationException("Azure OpenAI response 'content' field is null.");

        _logger.LogDebug("Received Azure OpenAI response length={Length}", text.Length);
        return text;
    }
}
