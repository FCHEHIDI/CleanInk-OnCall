using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.ValueObjects;

/// <summary>
/// Represents a French Numéro de Sécurité Sociale (NIR).
/// Format: 15 digits — [1|2][YY][MM][dpt][commune][ordre][key]
/// This value object is a core PII field — must be encrypted at rest (HDS compliance).
/// </summary>
public sealed class NirNumber : ValueObject
{
    /// <summary>The raw 15-digit NIR string.</summary>
    public string Value { get; }

    private NirNumber(string value) => Value = value;

    /// <summary>
    /// Creates a validated <see cref="NirNumber"/>.
    /// </summary>
    /// <param name="raw">Raw NIR string (spaces and dashes stripped automatically).</param>
    /// <returns>A <see cref="Result{NirNumber}"/> or a validation error.</returns>
    public static Result<NirNumber> Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Error.Validation(nameof(NirNumber), "NIR is required.");

        // Strip spaces and dashes that are commonly added for readability.
        var normalized = raw.Replace(" ", "").Replace("-", "");

        // Corse departments use 2A/2B — replace with 19 for numeric-only check.
        var numeric = normalized.Replace("2A", "19").Replace("2B", "19");

        if (numeric.Length != 15 || !numeric.All(char.IsDigit))
            return Error.Validation(nameof(NirNumber), "NIR must be exactly 15 digits.");

        if (!ValidateLuhnKey(numeric))
            return Error.Validation(nameof(NirNumber), "NIR control key is invalid.");

        return new NirNumber(normalized);
    }

    /// <summary>
    /// Validates the NIR using the INSEE control key algorithm.
    /// Key = 97 - (number mod 97), where key is the last 2 digits.
    /// </summary>
    private static bool ValidateLuhnKey(string numeric)
    {
        if (!long.TryParse(numeric[..13], out var number)) return false;
        if (!int.TryParse(numeric[13..], out var key)) return false;
        return 97 - (number % 97) == key;
    }

    /// <summary>
    /// Returns a masked representation safe for logging: e.g. "1 85 05 *** *** ** **"
    /// </summary>
    public string Masked() =>
        Value.Length == 15 ? $"{Value[0]} {Value[1..3]} {Value[3..5]} *** *** ** **" : "***";

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc/>
    public override string ToString() => Masked();
}
