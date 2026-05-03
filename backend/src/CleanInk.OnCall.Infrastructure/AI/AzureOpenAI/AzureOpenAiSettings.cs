namespace CleanInk.OnCall.Infrastructure.AI.AzureOpenAI;

/// <summary>
/// Configuration settings for the Azure OpenAI service.
/// Bind from appsettings section "AzureOpenAI".
/// </summary>
public sealed class AzureOpenAiSettings
{
    /// <summary>Section name in appsettings.json.</summary>
    public const string SectionName = "AzureOpenAI";

    /// <summary>Gets or sets the Azure OpenAI API key (required).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Gets or sets the Azure OpenAI resource endpoint, e.g. https://my-resource.openai.azure.com.</summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Gets or sets the deployment name (model alias in Azure portal).</summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>Gets or sets the API version query parameter.</summary>
    public string ApiVersion { get; set; } = "2024-02-01";

    /// <summary>Gets or sets the maximum tokens the model may generate.</summary>
    public int MaxTokens { get; set; } = 4096;
}
