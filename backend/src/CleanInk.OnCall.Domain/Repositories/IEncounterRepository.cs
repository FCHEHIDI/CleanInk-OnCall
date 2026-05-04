using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="Encounter"/> aggregate persistence.
/// All queries are scoped to the current tenant schema.
/// </summary>
public interface IEncounterRepository
{
    Task<Encounter?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Encounter>> GetByPatientAsync(
        Guid patientId,
        EncounterStatus? status = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<Encounter>> GetByPractitionerAsync(
        Guid practitionerId,
        EncounterStatus? status = null,
        CancellationToken ct = default);

    Task AddAsync(Encounter encounter, CancellationToken ct = default);
    void Update(Encounter encounter);

    /// <summary>Persists all pending changes.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
