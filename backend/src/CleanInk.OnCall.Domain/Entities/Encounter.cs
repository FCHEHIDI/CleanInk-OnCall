using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;
using CleanInk.OnCall.Shared.Fhir;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// FHIR R4 Encounter — clinical aggregate root representing a patient-practitioner interaction.
///
/// FHIR mapping: https://www.hl7.org/fhir/encounter.html
///
/// Domain invariants:
/// - An Encounter in "Finished" status is IMMUTABLE — only addendum notes may be appended.
/// - The subject (patient) must have given consent before an Encounter can be created.
/// - An Encounter must be linked to a Patient and at least one Practitioner.
/// - Class determines the care setting: "AMB" (ambulatory), "EMER" (emergency), "IMP" (inpatient).
/// </summary>
public sealed class Encounter : Entity<Guid>
{
    private Encounter() { }

    // ── FHIR Encounter.identifier ────────────────────────────────────────────

    /// <summary>Internal identifier for this encounter.</summary>
    public List<FhirIdentifier> Identifiers { get; private set; } = [];

    // ── FHIR Encounter.status ────────────────────────────────────────────────

    /// <summary>Current status of the encounter.</summary>
    public EncounterStatus Status { get; private set; }

    // ── FHIR Encounter.class ─────────────────────────────────────────────────

    /// <summary>
    /// Classification of the encounter.
    /// FHIR v3 ActCode: "AMB" | "EMER" | "IMP" | "HH" (home health).
    /// </summary>
    public CodeableConcept EncounterClass { get; private set; } = default!;

    // ── FHIR Encounter.type ──────────────────────────────────────────────────

    /// <summary>Specific type of encounter (SNOMED, CCAM, or internal code).</summary>
    public CodeableConcept? Type { get; private set; }

    // ── FHIR Encounter.subject ───────────────────────────────────────────────

    /// <summary>Patient ID (subject of the encounter).</summary>
    public Guid PatientId { get; private set; }

    // ── FHIR Encounter.participant ───────────────────────────────────────────

    /// <summary>ID of the responsible practitioner.</summary>
    public Guid PractitionerId { get; private set; }

    // ── FHIR Encounter.period ────────────────────────────────────────────────

    /// <summary>When the encounter occurred.</summary>
    public Period Period { get; private set; } = default!;

    // ── FHIR Encounter.reasonCode ────────────────────────────────────────────

    /// <summary>Reason for the encounter (chief complaint or diagnosis code).</summary>
    public string? ReasonText { get; private set; }

    /// <summary>CIM-10 / SNOMED coded reason.</summary>
    public CodeableConcept? ReasonCode { get; private set; }

    // ── Clinical notes ───────────────────────────────────────────────────────

    /// <summary>
    /// Structured clinical note added by the practitioner.
    /// Append-only once Encounter is Finished (no edit — addendum only).
    /// </summary>
    public string? ClinicalNote { get; private set; }

    // ── Metadata ─────────────────────────────────────────────────────────────

    /// <summary>UTC timestamp of creation.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>UTC timestamp of last update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Factory — creates a new in-progress encounter.
    /// Raises <see cref="EncounterStartedEvent"/>.
    /// </summary>
    public static Result<Encounter> Start(
        Guid patientId,
        Guid practitionerId,
        string encounterClass,
        string? reasonText = null)
    {
        if (patientId == Guid.Empty)
            return Error.Validation(nameof(PatientId), "Patient ID is required.");

        if (practitionerId == Guid.Empty)
            return Error.Validation(nameof(PractitionerId), "Practitioner ID is required.");

        var validClasses = new[] { "AMB", "EMER", "IMP", "HH" };
        if (!validClasses.Contains(encounterClass?.ToUpperInvariant()))
            return Error.Validation(nameof(EncounterClass),
                $"Encounter class must be one of: {string.Join(", ", validClasses)}.");

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var encounter = new Encounter
        {
            Id = id,
            Identifiers = [FhirIdentifier.Internal(id)],
            Status = EncounterStatus.InProgress,
            EncounterClass = CodeableConcept.Internal(encounterClass!.ToUpperInvariant(), encounterClass),
            PatientId = patientId,
            PractitionerId = practitionerId,
            Period = Period.StartingNow(),
            ReasonText = reasonText?.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
        };

        encounter.RaiseDomainEvent(new EncounterStartedEvent(id, patientId, practitionerId, now));

        return encounter;
    }

    /// <summary>
    /// Finishes the encounter and records the clinical note.
    /// Once finished, the encounter status is IMMUTABLE.
    /// </summary>
    /// <param name="clinicalNote">Mandatory closing note from the practitioner.</param>
    public Result Finish(string clinicalNote)
    {
        if (Status == EncounterStatus.Finished)
            return Result.Failure(new Error("Encounter.AlreadyFinished", "This encounter is already finished."));

        if (Status == EncounterStatus.Cancelled)
            return Result.Failure(new Error("Encounter.Cancelled", "A cancelled encounter cannot be finished."));

        if (string.IsNullOrWhiteSpace(clinicalNote))
            return Result.Failure(Error.Validation(nameof(ClinicalNote), "A clinical note is required to close an encounter."));

        Status = EncounterStatus.Finished;
        ClinicalNote = clinicalNote.Trim();
        Period = new Period(Period.Start, DateTime.UtcNow);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new EncounterFinishedEvent(Id, PatientId, PractitionerId, DateTime.UtcNow));

        return Result.Success;
    }

    /// <summary>
    /// Cancels the encounter. A finished encounter cannot be cancelled.
    /// </summary>
    public Result Cancel(string reason)
    {
        if (Status == EncounterStatus.Finished)
            return Result.Failure(new Error("Encounter.Immutable", "A finished encounter cannot be cancelled."));

        Status = EncounterStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success;
    }

    /// <summary>
    /// Appends an addendum to a finished encounter (e.g. lab results received later).
    /// Does not re-open the encounter.
    /// </summary>
    public void AppendAddendum(string addendum)
    {
        if (string.IsNullOrWhiteSpace(addendum)) return;

        ClinicalNote = ClinicalNote is null
            ? $"[ADDENDUM {DateTime.UtcNow:u}] {addendum}"
            : $"{ClinicalNote}\n[ADDENDUM {DateTime.UtcNow:u}] {addendum}";

        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>FHIR Encounter status codes.</summary>
public enum EncounterStatus
{
    Planned = 0,
    Arrived = 1,
    Triaged = 2,
    InProgress = 3,
    OnLeave = 4,
    Finished = 5,
    Cancelled = 6,
    Unknown = 7,
}
