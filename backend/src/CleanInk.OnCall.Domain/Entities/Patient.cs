using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;
using CleanInk.OnCall.Shared.Fhir;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// FHIR R4 Patient — clinical aggregate root.
///
/// FHIR mapping: https://www.hl7.org/fhir/patient.html
///
/// Domain invariants:
/// - Patient is NEVER physically deleted (pseudonymisation only via <see cref="Pseudonymize"/>).
/// - NIR (Numéro de Sécurité Sociale) must pass Luhn check if provided.
/// - Clinical data is accessible only to ClinicalRoles + emergency override (with audit).
/// - <see cref="ConsentGiven"/> must be true before any clinical data can be stored.
///
/// RGPD / HDS:
/// - NIR and phone are encrypted at the persistence layer (AES-256-GCM via EncryptionService).
/// - Email is pseudonymized in audit logs and research exports.
/// - Soft-delete via <see cref="IsPseudonymized"/> — actual data wiped, identifiers replaced.
///
/// FHIR identifiers:
/// - <see cref="Identifiers"/> carries FHIR FhirIdentifier records (INS, internal, etc.).
/// - The FHIR Resource "id" maps to <see cref="Entity{Guid}.Id"/>.
/// </summary>
public sealed class Patient : Entity<Guid>
{
    private Patient() { }

    // ── FHIR Patient.identifier ─────────────────────────────────────────────

    /// <summary>
    /// List of identifiers for this patient.
    /// Always includes an internal GUID identifier.
    /// May include INS (Identité Nationale de Santé) once verified.
    /// </summary>
    public List<FhirIdentifier> Identifiers { get; private set; } = [];

    // ── FHIR Patient.name ───────────────────────────────────────────────────

    /// <summary>
    /// Patient name(s). First entry is "official" (état civil).
    /// Stored uppercase (Family) + capitalized (Given) per French standards.
    /// </summary>
    public List<HumanName> Names { get; private set; } = [];

    // ── FHIR Patient.gender ─────────────────────────────────────────────────

    /// <summary>Administrative gender: "male" | "female" | "other" | "unknown".</summary>
    public string Gender { get; private set; } = "unknown";

    // ── FHIR Patient.birthDate ──────────────────────────────────────────────

    /// <summary>Date of birth. Required for patient identification.</summary>
    public DateOnly DateOfBirth { get; private set; }

    // ── FHIR Patient.identifier — NIR (encrypted at rest) ──────────────────

    /// <summary>
    /// French NIR (Numéro de Sécurité Sociale) — 15 digits.
    /// Encrypted at rest (AES-256-GCM). Never logged or serialized plaintext.
    /// </summary>
    public string? NirEncrypted { get; private set; }

    // ── FHIR Patient.telecom ────────────────────────────────────────────────

    /// <summary>
    /// Contact points (phone, email). Stored as JSON.
    /// Phone values are encrypted at rest.
    /// </summary>
    public List<ContactPoint> ContactPoints { get; private set; } = [];

    // ── Consent ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Whether the patient has given explicit RGPD consent for data processing.
    /// Required before any clinical data can be stored.
    /// Separate Consent resource tracks the full consent lifecycle.
    /// </summary>
    public bool ConsentGiven { get; private set; }

    /// <summary>UTC timestamp when consent was given.</summary>
    public DateTime? ConsentGivenAt { get; private set; }

    // ── Metadata ─────────────────────────────────────────────────────────────

    /// <summary>UTC timestamp when the patient record was created.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>UTC timestamp of the last record update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Pseudonymisation flag. When true, all identifying data has been replaced
    /// with opaque tokens. Record remains for statistical/audit continuity.
    /// </summary>
    public bool IsPseudonymized { get; private set; }

    /// <summary>
    /// FHIR Patient official name (first entry with use="official").
    /// </summary>
    public HumanName? OfficialName => Names.FirstOrDefault(n => n.Use == "official");

    /// <summary>
    /// Factory method — registers a new patient with consent.
    /// Raises <see cref="PatientRegisteredEvent"/>.
    /// </summary>
    /// <param name="lastName">Family name (stored uppercase per French état civil).</param>
    /// <param name="firstName">Given name.</param>
    /// <param name="dateOfBirth">Must be in the past.</param>
    /// <param name="gender">FHIR gender: "male" | "female" | "other" | "unknown".</param>
    /// <param name="phoneNumber">Optional phone (E.164, will be encrypted).</param>
    /// <param name="email">Optional email.</param>
    /// <param name="nirEncrypted">Optional NIR — must already be AES-encrypted by the caller.</param>
    /// <returns>A <see cref="Result{Patient}"/> or a validation error.</returns>
    public static Result<Patient> Register(
        string lastName,
        string firstName,
        DateOnly dateOfBirth,
        string gender = "unknown",
        string? phoneNumber = null,
        string? email = null,
        string? nirEncrypted = null)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation(nameof(lastName), "Last name is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation(nameof(firstName), "First name is required.");

        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow))
            return Error.Validation(nameof(dateOfBirth), "Date of birth must be in the past.");

        var validGenders = new[] { "male", "female", "other", "unknown" };
        if (!validGenders.Contains(gender))
            return Error.Validation(nameof(gender), $"Gender must be one of: {string.Join(", ", validGenders)}.");

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var contactPoints = new List<ContactPoint>();
        if (!string.IsNullOrWhiteSpace(phoneNumber))
            contactPoints.Add(ContactPoint.Mobile(phoneNumber));
        if (!string.IsNullOrWhiteSpace(email))
            contactPoints.Add(ContactPoint.Email(email));

        var patient = new Patient
        {
            Id = id,
            Identifiers = [FhirIdentifier.Internal(id)],
            Names = [HumanName.Official(lastName.Trim().ToUpperInvariant(), firstName.Trim())],
            Gender = gender,
            DateOfBirth = dateOfBirth,
            NirEncrypted = nirEncrypted,
            ContactPoints = contactPoints,
            ConsentGiven = false,
            CreatedAt = now,
            UpdatedAt = now,
        };

        patient.RaiseDomainEvent(new PatientRegisteredEvent(id, now));

        return patient;
    }

    /// <summary>
    /// Records explicit RGPD consent. Required before storing clinical data.
    /// </summary>
    public void RecordConsent()
    {
        ConsentGiven = true;
        ConsentGivenAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PatientConsentGivenEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Adds or replaces the INS identifier after official verification.
    /// </summary>
    /// <param name="insValue">INS value from the INSi connector.</param>
    public void SetInsIdentifier(string insValue)
    {
        Identifiers.RemoveAll(i => i.System == FhirIdentifier.Systems.Ins);
        Identifiers.Add(FhirIdentifier.Ins(insValue));
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Pseudonymizes the patient record (RGPD Article 17 — right to erasure).
    /// Replaces all PII with opaque tokens. Record remains for audit continuity.
    /// </summary>
    public void Pseudonymize()
    {
        if (IsPseudonymized) return;

        var pseudoId = Guid.NewGuid().ToString("N")[..8];
        Names = [HumanName.Official($"PSEUDO-{pseudoId}", "PSEUDO")];
        ContactPoints = [];
        NirEncrypted = null;
        Identifiers.RemoveAll(i => i.System != FhirIdentifier.Systems.CleanInkInternal);
        IsPseudonymized = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PatientPseudonymizedEvent(Id, DateTime.UtcNow));
    }
}
