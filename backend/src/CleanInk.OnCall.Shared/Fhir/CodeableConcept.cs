using System.Text.Json.Serialization;

namespace CleanInk.OnCall.Shared.Fhir;

/// <summary>
/// FHIR R4 Coding — a code within a terminology system.
/// </summary>
/// <param name="System">URI of the code system (e.g. SNOMED, CCAM, LOINC).</param>
/// <param name="Code">The symbol within the system.</param>
/// <param name="Display">Human-readable representation.</param>
public sealed record Coding(string System, string Code, string? Display = null)
{
    public static class Systems
    {
        public const string Snomed = "http://snomed.info/sct";
        public const string Loinc  = "http://loinc.org";
        public const string Ccam   = "https://www.ameli.fr/ccam";
        public const string Ngap   = "https://www.ameli.fr/ngap";
        public const string Icd10  = "http://hl7.org/fhir/sid/icd-10";
        public const string CleanInkEncounterType = "urn:oid:fr.cleanink.encounter-type";
        public const string CleanInkDocType       = "urn:oid:fr.cleanink.document-type";
    }
}

/// <summary>
/// FHIR R4 CodeableConcept — a concept represented by a code (or several) with optional text.
/// Used instead of raw enums for any coded value that may need interoperability.
/// </summary>
/// <param name="Codings">One or more codings from different systems.</param>
/// <param name="Text">Plain-text representation (fallback or user-entered).</param>
public sealed record CodeableConcept
{
    [JsonConstructor]
    public CodeableConcept(Coding[] Codings, string? Text = null)
    {
        this.Codings = Codings;
        this.Text = Text;
    }

    public Coding[] Codings { get; init; }
    public string? Text { get; init; }
    /// <summary>Convenience constructor for a single coding.</summary>
    public CodeableConcept(Coding coding, string? text = null)
        : this([coding], text) { }

    /// <summary>Convenience constructor for an internal code.</summary>
    public static CodeableConcept Internal(string code, string display) =>
        new(new Coding(Coding.Systems.CleanInkEncounterType, code, display));
}
