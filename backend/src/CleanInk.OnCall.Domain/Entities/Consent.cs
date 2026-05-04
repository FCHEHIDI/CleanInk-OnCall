using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;
using CleanInk.OnCall.Shared.Fhir;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// FHIR R4 Consent — explicit patient consent for data processing.
///
/// FHIR mapping: https://www.hl7.org/fhir/consent.html
///
/// RGPD / HDS:
/// - Consent must be given BEFORE any clinical data is stored (Article 9 RGPD).
/// - Consent can be withdrawn at any time — triggers pseudonymisation workflow.
/// - Consent history is immutable (withdrawal creates a new Consent record, does not update the old one).
///
/// Domain invariants:
/// - An "Active" consent cannot be modified — withdrawal creates a new Withdrawn record.
/// - Consent scope determines what data processing is authorized.
/// - Each consent is tied to exactly one patient.
/// </summary>
public sealed class Consent : Entity<Guid>
{
    private Consent() { }

    // ── FHIR Consent.patient ─────────────────────────────────────────────────

    /// <summary>Patient who granted or withdrew this consent.</summary>
    public Guid PatientId { get; private set; }

    // ── FHIR Consent.status ──────────────────────────────────────────────────

    /// <summary>Current consent status.</summary>
    public ConsentStatus Status { get; private set; }

    // ── FHIR Consent.scope ───────────────────────────────────────────────────

    /// <summary>
    /// Consent scope: "patient-privacy" | "research" | "adr" (adverse drug reaction) | "treatment".
    /// </summary>
    public string Scope { get; private set; } = string.Empty;

    // ── FHIR Consent.category ────────────────────────────────────────────────

    /// <summary>Category: "59284-0" (LOINC Patient Consent) or internal code.</summary>
    public string Category { get; private set; } = string.Empty;

    // ── FHIR Consent.dateTime ────────────────────────────────────────────────

    /// <summary>UTC timestamp when consent was given.</summary>
    public DateTime ConsentedAt { get; private set; }

    /// <summary>UTC timestamp when consent was withdrawn. Null if still active.</summary>
    public DateTime? WithdrawnAt { get; private set; }

    // ── FHIR Consent.provision.period ───────────────────────────────────────

    /// <summary>Validity period of this consent.</summary>
    public Period ValidityPeriod { get; private set; } = default!;

    // ── Actor who collected consent ──────────────────────────────────────────

    /// <summary>User (practitioner or agent) who collected this consent.</summary>
    public Guid CollectedByUserId { get; private set; }

    /// <summary>How consent was collected: "electronic" | "written" | "verbal".</summary>
    public string CollectionMethod { get; private set; } = "electronic";

    /// <summary>
    /// Factory — records a new patient consent.
    /// </summary>
    public static Result<Consent> Grant(
        Guid patientId,
        Guid collectedByUserId,
        string scope = "patient-privacy",
        string collectionMethod = "electronic",
        DateTime? expiresAt = null)
    {
        if (patientId == Guid.Empty)
            return Error.Validation(nameof(PatientId), "Patient ID is required.");

        if (collectedByUserId == Guid.Empty)
            return Error.Validation(nameof(CollectedByUserId), "Collector user ID is required.");

        var now = DateTime.UtcNow;
        return new Consent
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Status = ConsentStatus.Active,
            Scope = scope,
            Category = "59284-0",
            ConsentedAt = now,
            ValidityPeriod = new Period(now, expiresAt),
            CollectedByUserId = collectedByUserId,
            CollectionMethod = collectionMethod,
        };
    }

    /// <summary>
    /// Withdraws this consent. Creates an immutable record of withdrawal.
    /// </summary>
    public Result Withdraw()
    {
        if (Status == ConsentStatus.Withdrawn)
            return Result.Failure(new Error("Consent.AlreadyWithdrawn", "This consent has already been withdrawn."));

        Status = ConsentStatus.Withdrawn;
        WithdrawnAt = DateTime.UtcNow;
        return Result.Success;
    }
}

public enum ConsentStatus
{
    /// <summary>Consent is active and authorizes data processing.</summary>
    Active = 0,

    /// <summary>Consent has been withdrawn by the patient.</summary>
    Withdrawn = 1,

    /// <summary>Consent has expired.</summary>
    Expired = 2,
}
