namespace CleanInk.OnCall.Domain.Events;

/// <summary>Raised when a new call is created in the system.</summary>
public sealed record CallCreatedEvent(
    Guid CallId,
    Guid? PatientId,
    string Subject,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>Raised when a call is assigned to a practitioner.</summary>
public sealed record CallAssignedEvent(
    Guid CallId,
    Guid PractitionerId,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>Raised when a call is resolved successfully.</summary>
public sealed record CallResolvedEvent(
    Guid CallId,
    Guid? PatientId,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>Raised when a call is escalated to a higher tier.</summary>
public sealed record CallEscalatedEvent(
    Guid CallId,
    Guid? PatientId,
    string Reason,
    DateTime OccurredAt) : IDomainEvent;

/// <summary>Raised when a call is cancelled.</summary>
public sealed record CallCancelledEvent(
    Guid CallId,
    Guid? PatientId,
    DateTime OccurredAt) : IDomainEvent;

