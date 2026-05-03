namespace CleanInk.OnCall.Infrastructure.AI.Claude;

/// <summary>
/// Configuration settings for the Anthropic Claude API.
/// Bind from appsettings section "Claude".
/// </summary>
public sealed class ClaudeSettings
{
    /// <summary>Section name in appsettings.json.</summary>
    public const string SectionName = "Claude";

    /// <summary>Gets or sets the Anthropic API key (required).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Gets or sets the Claude model identifier.</summary>
    public string Model { get; set; } = "claude-3-5-sonnet-20241022";

    /// <summary>Gets or sets the Anthropic API base URL.</summary>
    public string BaseUrl { get; set; } = "https://api.anthropic.com";

    /// <summary>Gets or sets the maximum tokens the model may generate.</summary>
    public int MaxTokens { get; set; } = 4096;
}
