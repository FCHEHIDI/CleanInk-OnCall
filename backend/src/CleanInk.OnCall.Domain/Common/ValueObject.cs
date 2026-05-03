namespace CleanInk.OnCall.Domain.Common;

/// <summary>
/// Base class for all Value Objects in the domain.
/// Equality is based on the combination of all component values,
/// not on object identity — a core DDD principle.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns all components that participate in equality comparison.
    /// Implement by yielding each field: <c>yield return Field;</c>
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc/>
    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is ValueObject other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ (y << 5) + y);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
