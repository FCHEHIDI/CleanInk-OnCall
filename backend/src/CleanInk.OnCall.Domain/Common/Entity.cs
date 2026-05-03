using CleanInk.OnCall.Domain.Events;

namespace CleanInk.OnCall.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides a typed identity, value-equality,
/// and a domain events collection for raising events within aggregate roots.
///
/// Domain events are collected here and dispatched AFTER persistence by the
/// infrastructure layer (EF SaveChanges interceptor), ensuring consistency between
/// state changes and downstream notifications.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class Entity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Gets the unique identifier of this entity.</summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Read-only snapshot of uncommitted domain events.
    /// Infrastructure reads this after SaveChanges to publish via MediatR.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Raises a domain event and adds it to the uncommitted collection.
    /// Only call from within aggregate root methods.
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears the domain events collection after they have been dispatched.
    /// Called by the infrastructure layer post-publication.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>Determines equality by <see cref="Id"/>.</summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id.Equals(other.Id);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>Equality operator delegating to <see cref="Equals(object?)"/>.</summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
