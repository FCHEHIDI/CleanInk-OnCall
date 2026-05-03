using System.Text.RegularExpressions;
using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.ValueObjects;

/// <summary>
/// Represents a validated email address.
/// Stored in lowercase — two emails that differ only in case are considered equal.
/// PII field — must be protected in logs and pseudonymized for research exports.
/// </summary>
public sealed class EmailAddress : ValueObject
{
    private static readonly Regex EmailPattern = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>Normalised lowercase email address.</summary>
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    /// <summary>
    /// Creates a validated <see cref="EmailAddress"/>.
    /// </summary>
    /// <param name="raw">Raw email string. Automatically trimmed and lowercased.</param>
    /// <returns><see cref="Result{EmailAddress}"/> or a validation error.</returns>
    public static Result<EmailAddress> Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Error.Validation(nameof(EmailAddress), "Email address is required.");

        var normalized = raw.Trim().ToLowerInvariant();

        if (normalized.Length > 254)
            return Error.Validation(nameof(EmailAddress), "Email address cannot exceed 254 characters.");

        if (!EmailPattern.IsMatch(normalized))
            return Error.Validation(nameof(EmailAddress), $"'{raw}' is not a valid email address.");

        return new EmailAddress(normalized);
    }

    /// <summary>
    /// Returns a masked version safe for logs: "f***@domain.tld"
    /// </summary>
    public string Masked()
    {
        var atIdx = Value.IndexOf('@');
        if (atIdx <= 0) return "***@***";
        return $"{Value[0]}***{Value[atIdx..]}";
    }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc/>
    public override string ToString() => Masked();
}
