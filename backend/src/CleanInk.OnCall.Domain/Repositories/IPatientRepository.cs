using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for the <see cref="Patient"/> aggregate root.
/// All methods are async to support database I/O without blocking.
///
/// Implementations must handle NIR and Phone decryption transparently,
/// so callers always receive a fully hydrated <see cref="Patient"/> aggregate.
/// </summary>
public interface IPatientRepository
{
    /// <summary>Retrieves a patient by their unique identifier.</summary>
    /// <param name="id">Patient GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The patient, or <c>null</c> if not found.</returns>
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for patients by name fragment (case-insensitive).
    /// Returns only non-archived patients.
    /// </summary>
    /// <param name="nameFragment">Partial last name or first name.</param>
    /// <param name="pageNumber">1-based page number.</param>
    /// <param name="pageSize">Number of results per page (max 50).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(
        string nameFragment,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Persists a new patient record.</summary>
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing patient as modified (tracked entity — no return needed).</summary>
    void Update(Patient patient);

    /// <summary>Persists all pending changes.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
