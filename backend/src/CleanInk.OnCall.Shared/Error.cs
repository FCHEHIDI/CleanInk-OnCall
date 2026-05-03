namespace CleanInk.OnCall.Shared;

/// <summary>
/// Represents a domain error with a code and description.
/// </summary>
/// <param name="Code">Unique error code (e.g. "Call.NotFound").</param>
/// <param name="Description">Human-readable description of the error.</param>
public record Error(string Code, string Description)
{
    /// <summary>Represents the absence of an error (success path).</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Generic null/not-found error.</summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified value was null.");

    /// <summary>Creates a not-found error for a given resource.</summary>
    /// <param name="resource">Name of the resource (e.g. "Call").</param>
    /// <param name="id">Identifier that was not found.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error NotFound(string resource, object id) =>
        new($"{resource}.NotFound", $"{resource} with id '{id}' was not found.");

    /// <summary>Creates a validation error.</summary>
    /// <param name="field">Field name that failed validation.</param>
    /// <param name="message">Validation message.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);
}
