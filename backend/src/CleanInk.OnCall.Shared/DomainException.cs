namespace CleanInk.OnCall.Shared;

/// <summary>
/// Exception thrown when a domain invariant is violated.
/// Use this to surface business rule violations from the Domain layer.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>Gets the structured domain error associated with this exception.</summary>
    public Error Error { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DomainException"/> with a domain error.
    /// </summary>
    /// <param name="error">The domain error that caused this exception.</param>
    public DomainException(Error error)
        : base(error.Description)
    {
        Error = error;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DomainException"/> with a code and message.
    /// </summary>
    /// <param name="code">Error code.</param>
    /// <param name="description">Human-readable description.</param>
    public DomainException(string code, string description)
        : this(new Error(code, description))
    {
    }
}
