using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Domain.ValueObjects;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Aggregate root representing a patient in the healthcare system.
///
/// RGPD / HDS considerations:
/// - <see cref="NirNumber"/> is encrypted at the persistence layer (AES-256).
/// - <see cref="PhoneNumber"/> is encrypted at the persistence layer.
/// - <see cref="Email"/> is pseudonymized in export queries.
/// - The aggregate never exposes raw NIR — only masked representation.
/// - Consent is tracked separately via the <see cref="ConsentStatus"/> flag.
/// </summary>
public sealed class Patient : Entity<Guid>
{
    // Private parameterless constructor required by EF Core.
    private Patient() { }

    /// <summary>Last name (nom de famille). Stored in uppercase per French standards.</summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>First name(s). Stored as-entered.</summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>Date of birth. Never null — required for patient identification.</summary>
    public DateOnly DateOfBirth { get; private set; }

    /// <summary>
    /// French NIR (Numéro de Sécurité Sociale).
    /// Stored as encrypted string in DB — decrypted by EncryptionService.
    /// Exposed via this value object for domain logic only.
    /// </summary>
    public NirNumber? Nir { get; private set; }

    /// <summary>
    /// Contact phone number in E.164 format.
    /// Encrypted at rest.
    /// </summary>
    public PhoneNumber? Phone { get; private set; }

    /// <summary>
    /// Contact email address.
    /// Pseudonymized in audit logs and research exports.
    /// </summary>
    public EmailAddress? Email { get; private set; }

    /// <summary>
    /// Indicates whether the patient has given explicit RGPD consent
    /// for data processing (required before storing health data).
    /// </summary>
    public ConsentStatus Consent { get; private set; }

    /// <summary>UTC timestamp when the patient record was created.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>UTC timestamp of the last profile update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Soft-delete flag. Archived patients are hidden from search results.</summary>
    public bool IsArchived { get; private set; }

    /// <summary>
    /// Factory method: registers a new patient record.
    /// Raises <see cref="PatientRegisteredEvent"/>.
    /// </summary>
    /// <param name="lastName">Last name (trimmed, uppercased).</param>
    /// <param name="firstName">First name (trimmed).</param>
    /// <param name="dateOfBirth">Must be in the past.</param>
    /// <param name="nir">Optional NIR (validated value object).</param>
    /// <param name="phone">Optional phone number (validated value object).</param>
    /// <param name="email">Optional email address (validated value object).</param>
    /// <returns>A <see cref="Result{Patient}"/> with the new aggregate or validation error.</returns>
    public static Result<Patient> Register(
        string lastName,
        string firstName,
        DateOnly dateOfBirth,
        NirNumber? nir = null,
        PhoneNumber? phone = null,
        EmailAddress? email = null)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation(nameof(LastName), "Last name is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation(nameof(FirstName), "First name is required.");

        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow))
            return Error.Validation(nameof(DateOfBirth), "Date of birth must be in the past.");

        var now = DateTime.UtcNow;
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            LastName = lastName.Trim().ToUpperInvariant(),
            FirstName = firstName.Trim(),
            DateOfBirth = dateOfBirth,
            Nir = nir,
            Phone = phone,
            Email = email,
            Consent = ConsentStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now,
            IsArchived = false
        };

        patient.RaiseDomainEvent(new PatientRegisteredEvent(patient.Id, now));
        return patient;
    }

    /// <summary>
    /// Records explicit RGPD consent from the patient.
    /// Consent must be collected before any health data processing.
    /// </summary>
    public Result GrantConsent()
    {
        if (Consent == ConsentStatus.Granted)
            return Result.Failure(Error.Validation(nameof(Consent), "Consent has already been granted."));

        Consent = ConsentStatus.Granted;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>
    /// Records RGPD consent withdrawal (right to withdraw — Art. 7 RGPD).
    /// Does NOT delete data — triggers a separate data review process.
    /// </summary>
    public Result WithdrawConsent()
    {
        if (Consent == ConsentStatus.Withdrawn)
            return Result.Failure(Error.Validation(nameof(Consent), "Consent has already been withdrawn."));

        Consent = ConsentStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>
    /// Archives the patient record (soft-delete / right to erasure — Art. 17 RGPD).
    /// Archived records are removed from search results and pseudonymized in exports.
    /// </summary>
    public Result Archive()
    {
        if (IsArchived)
            return Result.Failure(Error.Validation("Patient.AlreadyArchived", "Patient is already archived."));

        IsArchived = true;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>
    /// Updates contact information for the patient.
    /// </summary>
    public void UpdateContact(PhoneNumber? phone, EmailAddress? email)
    {
        Phone = phone;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// RGPD consent status for health data processing.
/// </summary>
public enum ConsentStatus
{
    /// <summary>Consent not yet collected — data cannot be processed.</summary>
    Pending = 0,

    /// <summary>Patient has explicitly given consent (Art. 6 RGPD).</summary>
    Granted = 1,

    /// <summary>Patient has withdrawn consent (Art. 7 RGPD).</summary>
    Withdrawn = 2
}
