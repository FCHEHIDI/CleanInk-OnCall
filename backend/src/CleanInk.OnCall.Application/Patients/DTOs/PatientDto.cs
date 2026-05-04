using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Application.Patients.DTOs;

/// <summary>
/// Data transfer object for a patient record returned by the API.
/// </summary>
/// <param name="Id">Patient identifier.</param>
/// <param name="LastName">Last name (uppercase).</param>
/// <param name="FirstName">First name.</param>
/// <param name="DateOfBirth">Date of birth (ISO 8601).</param>
/// <param name="Gender">FHIR gender string (male/female/other/unknown).</param>
/// <param name="NirEncrypted">Encrypted NIR stored server-side. Null if not provided.</param>
/// <param name="ConsentGiven">Whether RGPD consent has been recorded.</param>
/// <param name="IsPseudonymized">Whether PII has been pseudonymized.</param>
/// <param name="CreatedAt">UTC creation timestamp.</param>
public sealed record PatientDto(
    Guid Id,
    string LastName,
    string FirstName,
    DateOnly DateOfBirth,
    string Gender,
    bool NirEncrypted,
    bool ConsentGiven,
    bool IsPseudonymized,
    DateTime CreatedAt)
{
    /// <summary>Maps a <see cref="Patient"/> aggregate to a <see cref="PatientDto"/>.</summary>
    public static PatientDto FromEntity(Patient p) => new(
        p.Id,
        p.OfficialName?.Family ?? string.Empty,
        p.OfficialName?.Given.FirstOrDefault() ?? string.Empty,
        p.DateOfBirth,
        p.Gender,
        p.NirEncrypted is not null,
        p.ConsentGiven,
        p.IsPseudonymized,
        p.CreatedAt);
}
