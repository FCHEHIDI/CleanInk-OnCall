namespace CleanInk.OnCall.Domain.Events;

/// <summary>
/// Raised when a new call is created and enters the queue.
/// Triggers: AI auto-triage background job, audit log entry.
/// </summary>
/// <param name="CallId">ID of the newly created call.</param>
/// <param name="PatientId">Optional — ID of the identified patient.</param>
/// <param name="Subject">Call subject for triage classification.</param>
/// <param name="OccurredAt">UTC timestamp of creation.</param>
public sealed record CallCreatedEvent(
    Guid CallId,
    Guid? PatientId,
    string Subject,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>
/// Raised when a call is escalated to a senior clinician or doctor.
/// Triggers: notification to assigned médecin, audit log entry, urgency flag.
/// </summary>
/// <param name="CallId">ID of the escalated call.</param>
/// <param name="EscalatedBy">ID of the operator who triggered escalation.</param>
/// <param name="Reason">Clinical reason for escalation.</param>
/// <param name="OccurredAt">UTC timestamp of escalation.</param>
public sealed record CallEscalatedEvent(
    Guid CallId,
    Guid EscalatedBy,
    string Reason,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>
/// Raised when a call is resolved and closed.
/// Triggers: invoice creation, patient history update, audit log entry.
/// </summary>
/// <param name="CallId">ID of the closed call.</param>
/// <param name="ClosedBy">ID of the operator who closed the call.</param>
/// <param name="Summary">Mandatory clinical summary of the call outcome.</param>
/// <param name="OccurredAt">UTC timestamp of closure.</param>
public sealed record CallClosedEvent(
    Guid CallId,
    Guid ClosedBy,
    string Summary,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>
/// Raised when a call is assigned to an operator.
/// </summary>
/// <param name="CallId">ID of the assigned call.</param>
/// <param name="OperatorId">ID of the operator taking charge.</param>
/// <param name="OccurredAt">UTC timestamp of assignment.</param>
public sealed record CallAssignedEvent(
    Guid CallId,
    Guid OperatorId,
    DateTime OccurredAt) : IDomainEvent;
