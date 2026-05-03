namespace CleanInk.OnCall.Infrastructure.AI;

/// <summary>
/// Abstraction for AI language model services.
/// Implementations may target Claude, Azure OpenAI, or any compatible backend.
/// </summary>
public interface IAiService
{
    /// <summary>
    /// Sends a single prompt and returns the model's text response.
    /// </summary>
    /// <param name="prompt">The full prompt to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The generated text from the model.</returns>
    Task<string> GenerateAsync(string prompt, CancellationToken ct = default);

    /// <summary>
    /// Sends a chat message and returns the model's text reply.
    /// </summary>
    /// <param name="message">The user message to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The model's chat reply.</returns>
    Task<string> ChatAsync(string message, CancellationToken ct = default);
}
