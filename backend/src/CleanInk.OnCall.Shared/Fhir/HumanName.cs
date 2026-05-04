namespace CleanInk.OnCall.Shared.Fhir;

/// <summary>
/// FHIR R4 HumanName — a human name with support for multiple representations.
/// </summary>
/// <param name="Family">Family name / last name.</param>
/// <param name="Given">Given names (first name, middle names).</param>
/// <param name="Prefix">Honorifics (Dr, Pr, M, Mme...).</param>
/// <param name="Suffix">Suffixes (Jr, III...).</param>
/// <param name="Use">How this name is used: "official" | "usual" | "nickname" | "maiden".</param>
public sealed record HumanName(
    string Family,
    string[] Given,
    string[]? Prefix = null,
    string[]? Suffix = null,
    string? Use = null)
{
    /// <summary>Returns a display string (e.g. "Dr Jean Dupont").</summary>
    public string Display =>
        string.Join(" ", new[]
        {
            Prefix is { Length: > 0 } ? string.Join(" ", Prefix) : null,
            string.Join(" ", Given),
            Family,
        }.Where(p => p is not null));

    public static HumanName Official(string family, params string[] given) =>
        new(family, given, Use: "official");

    public static HumanName WithTitle(string title, string family, params string[] given) =>
        new(family, given, Prefix: [title], Use: "official");
}
