namespace CleanInk.OnCall.Domain.Events;

/// <summary>
/// Marker interface for all domain events.
/// Domain events are raised by aggregate roots when a significant business
/// state change occurs. They are collected on the entity and dispatched
/// AFTER persistence by the infrastructure layer to ensure consistency
/// between the state change and downstream notifications.
///
/// Intentionally free of external dependencies (no MediatR reference here)
/// to keep the Domain layer pure — a fundamental Clean Architecture constraint.
/// </summary>
public interface IDomainEvent
{
    /// <summary>UTC timestamp when the event occurred.</summary>
    DateTime OccurredAt { get; }
}
