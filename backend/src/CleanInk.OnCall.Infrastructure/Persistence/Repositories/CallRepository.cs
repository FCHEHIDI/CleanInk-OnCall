using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ICallRepository"/>.
/// </summary>
public sealed class CallRepository : ICallRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<CallRepository> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CallRepository"/>.
    /// </summary>
    public CallRepository(AppDbContext db, ILogger<CallRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Call?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching call {CallId}", id);
        return await _db.Calls.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Call>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? patientId = null,
        Guid? assignedPractitionerId = null,
        CallStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _db.Calls.AsNoTracking();

        if (patientId.HasValue)
            query = query.Where(c => c.PatientId == patientId.Value);

        if (assignedPractitionerId.HasValue)
            query = query.Where(c => c.AssignedPractitionerId == assignedPractitionerId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(
        Guid? patientId = null,
        Guid? assignedPractitionerId = null,
        CallStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _db.Calls.AsNoTracking();

        if (patientId.HasValue)
            query = query.Where(c => c.PatientId == patientId.Value);

        if (assignedPractitionerId.HasValue)
            query = query.Where(c => c.AssignedPractitionerId == assignedPractitionerId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query.CountAsync(ct);
    }

    /// <inheritdoc/>
    public async Task AddAsync(Call call, CancellationToken ct = default)
    {
        await _db.Calls.AddAsync(call, ct);
    }

    /// <inheritdoc/>
    public void Update(Call call)
    {
        _db.Calls.Update(call);
    }
}
