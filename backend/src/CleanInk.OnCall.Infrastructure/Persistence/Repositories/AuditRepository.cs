using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanInk.OnCall.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuditRepository"/>.
/// Append-only: no Update or Delete operations are exposed.
/// </summary>
public sealed class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance of <see cref="AuditRepository"/>.</summary>
    public AuditRepository(AppDbContext context) => _context = context;

    /// <inheritdoc/>
    public async Task AddAsync(AuditEvent auditEvent, CancellationToken ct = default) =>
        await _context.AuditEvents.AddAsync(auditEvent, ct);

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<AuditEvent> auditEvents, CancellationToken ct = default) =>
        await _context.AuditEvents.AddRangeAsync(auditEvents, ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AuditEvent>> GetByResourceAsync(
        string resourceType,
        Guid resourceId,
        CancellationToken ct = default) =>
        await _context.AuditEvents
            .AsNoTracking()
            .Where(a => a.ResourceType == resourceType && a.ResourceId == resourceId)
            .OrderByDescending(a => a.RecordedAt)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AuditEvent>> GetByActorAsync(
        Guid actorId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default)
    {
        var query = _context.AuditEvents
            .AsNoTracking()
            .Where(a => a.ActorId == actorId);

        if (from.HasValue) query = query.Where(a => a.RecordedAt >= from.Value);
        if (to.HasValue) query = query.Where(a => a.RecordedAt <= to.Value);

        return await query.OrderByDescending(a => a.RecordedAt).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AuditEvent>> GetPendingEmergencyAccessReviewsAsync(
        Guid tenantId,
        CancellationToken ct = default) =>
        await _context.AuditEvents
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.IsEmergencyAccess)
            .OrderByDescending(a => a.RecordedAt)
            .ToListAsync(ct);
}

