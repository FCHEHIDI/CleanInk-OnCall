using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Application.Patients.DTOs;

/// <summary>
/// Data transfer object for a patient record returned by the API.
/// PII fields are masked by default when the caller is not the data owner.
/// </summary>
/// <param name="Id">Patient identifier.</param>
/// <param name="LastName">Last name (uppercase).</param>
/// <param name="FirstName">First name.</param>
/// <param name="DateOfBirth">Date of birth (ISO 8601).</param>
/// <param name="NirMasked">Masked NIR (e.g., "1 85 05 *** *** ** **"). Null if not provided.</param>
/// <param name="PhoneMasked">Masked phone (e.g., "+336****5678"). Null if not provided.</param>
/// <param name="EmailMasked">Masked email (e.g., "f***@example.com"). Null if not provided.</param>
/// <param name="Consent">Consent status: Pending, Granted, or Withdrawn.</param>
/// <param name="IsArchived">Whether the patient has been soft-deleted.</param>
/// <param name="CreatedAt">UTC creation timestamp.</param>
public sealed record PatientDto(
    Guid Id,
    string LastName,
    string FirstName,
    DateOnly DateOfBirth,
    string? NirMasked,
    string? PhoneMasked,
    string? EmailMasked,
    string Consent,
    bool IsArchived,
    DateTime CreatedAt)
{
    /// <summary>Maps a <see cref="Patient"/> aggregate to a <see cref="PatientDto"/>.</summary>
    public static PatientDto FromEntity(Patient p) => new(
        p.Id,
        p.LastName,
        p.FirstName,
        p.DateOfBirth,
        p.Nir?.Masked(),
        p.Phone?.Masked(),
        p.Email?.Masked(),
        p.Consent.ToString(),
        p.IsArchived,
        p.CreatedAt);
}
