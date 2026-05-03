using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.AI.Agents;

/// <summary>
/// Routes an input to the appropriate <see cref="IAgent"/> by name and returns the result.
/// </summary>
public sealed class AgentOrchestrator
{
    private readonly IReadOnlyDictionary<string, IAgent> _agents;
    private readonly ILogger<AgentOrchestrator> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AgentOrchestrator"/>.
    /// </summary>
    /// <param name="agents">All registered agents; keyed internally by <see cref="IAgent.Name"/>.</param>
    /// <param name="logger">Logger instance.</param>
    public AgentOrchestrator(IEnumerable<IAgent> agents, ILogger<AgentOrchestrator> logger)
    {
        _agents = agents.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        _logger = logger;
    }

    /// <summary>
    /// Executes the agent identified by <paramref name="agentName"/> with the given input.
    /// </summary>
    /// <param name="agentName">Agent name (e.g. "triage", "summary", "compliance").</param>
    /// <param name="input">The text to process.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The agent's text output.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no agent is registered under <paramref name="agentName"/>.
    /// </exception>
    public async Task<string> RunAsync(string agentName, string input, CancellationToken ct = default)
    {
        if (!_agents.TryGetValue(agentName, out var agent))
        {
            _logger.LogError("No agent registered with name '{AgentName}'", agentName);
            throw new KeyNotFoundException($"Agent '{agentName}' is not registered.");
        }

        _logger.LogInformation("Orchestrator dispatching to agent '{AgentName}'", agentName);
        return await agent.RunAsync(input, ct);
    }

    /// <summary>Gets the names of all registered agents.</summary>
    public IEnumerable<string> RegisteredAgents => _agents.Keys;
}
