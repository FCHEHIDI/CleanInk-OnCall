using CleanInk.OnCall.Domain.Events;

namespace CleanInk.OnCall.Application.Common.Interfaces;

/// <summary>
/// Abstraction for dispatching domain events after aggregate mutations are persisted.
///
/// The infrastructure layer (EF SaveChanges interceptor) implements this interface
/// using MediatR to fan-out each <see cref="IDomainEvent"/> to all registered handlers.
///
/// This indirection keeps the Application layer independent of MediatR's concrete types
/// while still allowing event-driven side effects (audit logging, notifications, projections).
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches all domain events from the provided entities.
    /// </summary>
    /// <param name="events">Domain events to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}
