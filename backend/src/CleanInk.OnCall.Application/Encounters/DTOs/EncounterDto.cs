using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Application.Encounters.DTOs;

/// <summary>
/// Data transfer object for an Encounter (FHIR R4 Encounter).
/// </summary>
/// <param name="Id">Encounter identifier.</param>
/// <param name="PatientId">Subject patient identifier.</param>
/// <param name="PractitionerId">Responsible practitioner identifier.</param>
/// <param name="Status">Current status (InProgress, Finished, Cancelled).</param>
/// <param name="EncounterClass">Classification code (AMB, EMER, IMP, HH).</param>
/// <param name="ReasonText">Free-text reason for the encounter.</param>
/// <param name="ClinicalNote">Practitioner clinical note (populated when Finished).</param>
/// <param name="PeriodStart">UTC start of the encounter.</param>
/// <param name="PeriodEnd">UTC end of the encounter (null if still in progress).</param>
/// <param name="CreatedAt">UTC creation timestamp.</param>
/// <param name="UpdatedAt">UTC last update timestamp.</param>
public sealed record EncounterDto(
    Guid Id,
    Guid PatientId,
    Guid PractitionerId,
    string Status,
    string EncounterClass,
    string? ReasonText,
    string? ClinicalNote,
    DateTime PeriodStart,
    DateTime? PeriodEnd,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    /// <summary>Maps an <see cref="Encounter"/> aggregate to an <see cref="EncounterDto"/>.</summary>
    public static EncounterDto FromEntity(Encounter e) => new(
        e.Id,
        e.PatientId,
        e.PractitionerId,
        e.Status.ToString(),
        e.EncounterClass.Codings.FirstOrDefault()?.Code ?? string.Empty,
        e.ReasonText,
        e.ClinicalNote,
        e.Period.Start,
        e.Period.End,
        e.CreatedAt,
        e.UpdatedAt);
}
