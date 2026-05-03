namespace CleanInk.OnCall.Infrastructure.AI.Agents;

/// <summary>
/// Contract for a single-purpose AI agent that executes a specific task.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// Gets the agent's unique name used for routing.
    /// Convention: lowercase, e.g. "triage", "summary", "compliance".
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Executes the agent's task on the given input text.
    /// </summary>
    /// <param name="input">The raw input to process (e.g. call transcription).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The agent's text output.</returns>
    Task<string> RunAsync(string input, CancellationToken ct = default);
}
