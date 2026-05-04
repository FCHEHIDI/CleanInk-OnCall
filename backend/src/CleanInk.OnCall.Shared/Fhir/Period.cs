namespace CleanInk.OnCall.Shared.Fhir;

/// <summary>
/// FHIR R4 Period — a time range with optional end date.
/// Used for encounter duration, appointment windows, consent validity, etc.
/// </summary>
/// <param name="Start">Start of the period (UTC).</param>
/// <param name="End">End of the period. Null if ongoing.</param>
public sealed record Period(DateTime Start, DateTime? End = null)
{
    /// <summary>Returns true if the period is currently active.</summary>
    public bool IsActive => End is null || End > DateTime.UtcNow;

    /// <summary>Duration in minutes, or null if the period has no end.</summary>
    public double? DurationMinutes => End.HasValue
        ? (End.Value - Start).TotalMinutes
        : null;

    /// <summary>Creates a period starting now.</summary>
    public static Period StartingNow() => new(DateTime.UtcNow);

    /// <summary>Creates a closed period.</summary>
    public static Period Closed(DateTime start, DateTime end) => new(start, end);
}
