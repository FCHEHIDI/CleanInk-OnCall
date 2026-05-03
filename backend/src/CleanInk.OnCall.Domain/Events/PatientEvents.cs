namespace CleanInk.OnCall.Domain.Events;

/// <summary>
/// Raised when a new patient record is registered in the system.
/// Triggers: RGPD consent workflow initiation, audit log entry.
/// </summary>
/// <param name="PatientId">ID of the newly registered patient.</param>
/// <param name="OccurredAt">UTC timestamp of registration.</param>
public sealed record PatientRegisteredEvent(
    Guid PatientId,
    DateTime OccurredAt) : IDomainEvent;
