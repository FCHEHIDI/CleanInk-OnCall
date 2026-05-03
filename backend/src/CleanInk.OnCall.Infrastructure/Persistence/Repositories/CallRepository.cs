using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using CleanInk.OnCall.Shared;
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
    /// <param name="db">The EF Core database context.</param>
    /// <param name="logger">Logger instance.</param>
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
    public async Task<PagedResult<Call>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? customerId = null,
        Guid? operatorId = null,
        CancellationToken ct = default)
    {
        var query = _db.Calls.AsNoTracking();

        if (customerId.HasValue)
            query = query.Where(c => c.CustomerId == customerId.Value);

        if (operatorId.HasValue)
            query = query.Where(c => c.OperatorId == operatorId.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Call>(items, page, pageSize, totalCount);
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

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
