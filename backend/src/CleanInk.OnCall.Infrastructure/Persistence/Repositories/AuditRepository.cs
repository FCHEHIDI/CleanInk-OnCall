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
    public async Task AddAsync(AuditLog entry, CancellationToken cancellationToken = default) =>
        await _context.AuditLogs.AddAsync(entry, cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entityType,
        string entityId,
        CancellationToken cancellationToken = default) =>
        await _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AuditLog>> GetByActorAsync(
        Guid actorId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.ActorId == actorId);

        if (from.HasValue) query = query.Where(a => a.OccurredAt >= from.Value);
        if (to.HasValue) query = query.Where(a => a.OccurredAt <= to.Value);

        return await query
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(cancellationToken);
    }
}
