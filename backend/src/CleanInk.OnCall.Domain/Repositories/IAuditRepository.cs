using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository for audit log entries.
/// Append-only: entries are never updated or deleted programmatically.
/// </summary>
public interface IAuditRepository
{
    /// <summary>
    /// Persists a new audit log entry (append-only).
    /// </summary>
    Task AddAsync(AuditLog entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves audit entries for a specific entity, ordered by most recent first.
    /// </summary>
    /// <param name="entityType">Entity type name (e.g. "Call", "Patient").</param>
    /// <param name="entityId">Entity ID as a string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entityType,
        string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all audit entries by a specific actor, ordered by most recent first.
    /// Used for CNIL compliance investigations.
    /// </summary>
    /// <param name="actorId">ID of the user whose actions are being reviewed.</param>
    /// <param name="from">Optional start of date range.</param>
    /// <param name="to">Optional end of date range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<AuditLog>> GetByActorAsync(
        Guid actorId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}
