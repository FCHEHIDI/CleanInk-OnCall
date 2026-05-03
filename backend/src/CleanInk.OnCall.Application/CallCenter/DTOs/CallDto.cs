namespace CleanInk.OnCall.Application.CallCenter.DTOs;

/// <summary>
/// Data transfer object representing a call returned by the API.
/// </summary>
/// <param name="Id">Unique identifier of the call.</param>
/// <param name="CustomerId">ID of the customer who placed the call.</param>
/// <param name="OperatorId">ID of the assigned operator, or null if unassigned.</param>
/// <param name="Subject">Short subject of the call.</param>
/// <param name="Description">Full description of the call.</param>
/// <param name="Status">Current status string (e.g. "Pending", "InProgress").</param>
/// <param name="AiTriageTag">AI-generated classification tag, or null.</param>
/// <param name="AiSummary">AI-generated summary, or null.</param>
/// <param name="CreatedAt">UTC creation timestamp.</param>
/// <param name="UpdatedAt">UTC last-update timestamp.</param>
public record CallDto(
    Guid Id,
    Guid CustomerId,
    Guid? OperatorId,
    string Subject,
    string Description,
    string Status,
    string? AiTriageTag,
    string? AiSummary,
    DateTime CreatedAt,
    DateTime UpdatedAt);
