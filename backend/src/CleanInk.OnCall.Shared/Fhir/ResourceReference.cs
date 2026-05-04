namespace CleanInk.OnCall.Shared.Fhir;

/// <summary>
/// FHIR R4 Reference — a reference to another FHIR resource.
/// Used to express relationships between resources without embedding full objects.
/// </summary>
/// <param name="Reference">Relative or absolute URL to the resource (e.g. "Patient/abc-123").</param>
/// <param name="Type">Resource type URI (e.g. "Patient", "Practitioner").</param>
/// <param name="Display">Human-readable text for display purposes.</param>
public sealed record ResourceReference(string Reference, string Type, string? Display = null)
{
    public static ResourceReference To(string type, Guid id, string? display = null) =>
        new($"{type}/{id:D}", type, display);

    public static ResourceReference ToPatient(Guid id, string? display = null) =>
        To("Patient", id, display);

    public static ResourceReference ToPractitioner(Guid id, string? display = null) =>
        To("Practitioner", id, display);

    public static ResourceReference ToEncounter(Guid id) =>
        To("Encounter", id);
}
