using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using CleanInk.OnCall.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IInvoiceRepository"/>.
/// </summary>
public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<InvoiceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="InvoiceRepository"/>.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    /// <param name="logger">Logger instance.</param>
    public InvoiceRepository(AppDbContext db, ILogger<InvoiceRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching invoice {InvoiceId}", id);
        return await _db.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<Invoice>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? customerId = null,
        CancellationToken ct = default)
    {
        var query = _db.Invoices.AsNoTracking();

        if (customerId.HasValue)
            query = query.Where(i => i.CustomerId == customerId.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Invoice>(items, page, pageSize, totalCount);
    }

    /// <inheritdoc/>
    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        await _db.Invoices.AddAsync(invoice, ct);
    }

    /// <inheritdoc/>
    public void Update(Invoice invoice)
    {
        _db.Invoices.Update(invoice);
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
