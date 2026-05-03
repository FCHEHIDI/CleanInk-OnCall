using System.Text.RegularExpressions;
using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.ValueObjects;

/// <summary>
/// Represents a validated telephone number in E.164 format.
/// Examples: +33612345678, +14155552671
/// PII field — must be masked in logs.
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex E164Pattern = new(@"^\+[1-9]\d{7,14}$", RegexOptions.Compiled);

    /// <summary>Phone number in E.164 format (e.g., +33612345678).</summary>
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    /// <summary>
    /// Creates a validated <see cref="PhoneNumber"/>.
    /// </summary>
    /// <param name="raw">Raw phone number. French 0X formats are automatically prefixed with +33.</param>
    /// <returns><see cref="Result{PhoneNumber}"/> with validated number or validation error.</returns>
    public static Result<PhoneNumber> Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Error.Validation(nameof(PhoneNumber), "Phone number is required.");

        var normalized = raw.Trim().Replace(" ", "").Replace(".", "").Replace("-", "");

        // Auto-convert French national format 0X... → +33X...
        if (normalized.StartsWith("0") && normalized.Length == 10)
            normalized = "+33" + normalized[1..];

        if (!E164Pattern.IsMatch(normalized))
            return Error.Validation(nameof(PhoneNumber), $"Phone number '{raw}' is not a valid E.164 format (+CCXXXXXXXXX).");

        return new PhoneNumber(normalized);
    }

    /// <summary>
    /// Returns a masked version safe for logging, e.g. "+336****5678".
    /// </summary>
    public string Masked() =>
        Value.Length > 6 ? $"{Value[..4]}****{Value[^4..]}" : "****";

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc/>
    public override string ToString() => Masked();
}
