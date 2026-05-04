namespace CleanInk.OnCall.Shared.Fhir;

/// <summary>
/// FHIR R4 Identifier — a value in a namespace.
/// Each entity can carry multiple identifiers from different systems
/// (e.g. internal GUID, INS, RPPS, FINESS).
/// </summary>
/// <param name="System">URI of the namespace (e.g. "https://annuaire.sante.fr/fhir/ins").</param>
/// <param name="Value">The identifier value within that system.</param>
/// <param name="Use">How this identifier is used: "official" | "usual" | "temp" | "secondary".</param>
public sealed record FhirIdentifier(string System, string Value, string? Use = null)
{
    // Well-known system URIs
    public static class Systems
    {
        public const string CleanInkInternal = "urn:oid:fr.cleanink.internal";
        public const string Ins              = "https://annuaire.sante.fr/fhir/ins";
        public const string Rpps             = "https://annuaire.sante.fr/fhir/rpps";
        public const string Adeli            = "https://annuaire.sante.fr/fhir/adeli";
        public const string Finess           = "https://finess.esante.gouv.fr";
        public const string Nir              = "urn:oid:fr.insee.nir";
    }

    public static FhirIdentifier Internal(Guid id) =>
        new(Systems.CleanInkInternal, id.ToString("D"), "usual");

    public static FhirIdentifier Ins(string value) =>
        new(Systems.Ins, value, "official");

    public static FhirIdentifier Rpps(string value) =>
        new(Systems.Rpps, value, "official");
}
