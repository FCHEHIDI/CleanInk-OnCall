using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Append-only repository for FHIR AuditEvent records.
/// Entries are NEVER updated or deleted — append-only by design and enforced at DB level.
/// Lives in the <c>cleanink_shared</c> schema for cross-tenant compliance queries.
/// </summary>
public interface IAuditRepository
{
    /// <summary>Persists a new audit event (append-only).</summary>
    Task AddAsync(AuditEvent auditEvent, CancellationToken ct = default);

    /// <summary>Bulk-appends multiple audit events (e.g. dispatched from domain event handlers).</summary>
    Task AddRangeAsync(IEnumerable<AuditEvent> auditEvents, CancellationToken ct = default);

    /// <summary>Retrieves audit events for a specific resource, ordered by most recent first.</summary>
    Task<IReadOnlyList<AuditEvent>> GetByResourceAsync(
        string resourceType,
        Guid resourceId,
        CancellationToken ct = default);

    /// <summary>Retrieves audit events by actor, ordered by most recent first. For CNIL compliance investigations.</summary>
    Task<IReadOnlyList<AuditEvent>> GetByActorAsync(
        Guid actorId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default);

    /// <summary>Retrieves all emergency access events pending review.</summary>
    Task<IReadOnlyList<AuditEvent>> GetPendingEmergencyAccessReviewsAsync(
        Guid tenantId,
        CancellationToken ct = default);
}
