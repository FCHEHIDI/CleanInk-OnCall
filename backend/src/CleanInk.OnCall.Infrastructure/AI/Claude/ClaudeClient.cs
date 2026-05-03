using System.Net.Http.Json;
using System.Text.Json;
using CleanInk.OnCall.Infrastructure.AI.Claude;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanInk.OnCall.Infrastructure.AI.Claude;

/// <summary>
/// Low-level HTTP client for the Anthropic Messages API.
/// POST /v1/messages with the configured model.
/// </summary>
public sealed class ClaudeClient
{
    private readonly HttpClient _http;
    private readonly ClaudeSettings _settings;
    private readonly ILogger<ClaudeClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>
    /// Initializes a new instance of <see cref="ClaudeClient"/>.
    /// </summary>
    /// <param name="http">Named HttpClient configured with the Anthropic base address and headers.</param>
    /// <param name="settings">Claude configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public ClaudeClient(
        HttpClient http,
        IOptions<ClaudeSettings> settings,
        ILogger<ClaudeClient> logger)
    {
        _http = http;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends a single user message to Claude and returns the text response.
    /// </summary>
    /// <param name="prompt">The text prompt to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The model's text response.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response body is missing expected fields.</exception>
    public async Task<string> SendAsync(string prompt, CancellationToken ct = default)
    {
        _logger.LogDebug("Sending prompt to Claude model={Model} length={Length}",
            _settings.Model, prompt.Length);

        var requestBody = new
        {
            model = _settings.Model,
            max_tokens = _settings.MaxTokens,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        using var response = await _http.PostAsJsonAsync("/v1/messages", requestBody, JsonOptions, ct);
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);

        var root = document.RootElement;

        if (!root.TryGetProperty("content", out var contentArray) ||
            contentArray.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Claude response missing 'content' array.");
        }

        var text = contentArray[0].GetProperty("text").GetString()
            ?? throw new InvalidOperationException("Claude response 'text' field is null.");

        _logger.LogDebug("Received Claude response length={Length}", text.Length);
        return text;
    }
}
