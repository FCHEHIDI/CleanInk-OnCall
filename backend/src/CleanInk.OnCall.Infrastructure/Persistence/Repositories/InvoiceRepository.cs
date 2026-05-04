using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
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

    /// <summary>Initializes a new instance of <see cref="InvoiceRepository"/>.</summary>
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
    public async Task<Invoice?> GetByReferenceAsync(string reference, CancellationToken ct = default)
        => await _db.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Reference == reference, ct);

    /// <inheritdoc/>
    public async Task<bool> ReferenceExistsAsync(string reference, CancellationToken ct = default)
        => await _db.Invoices.AnyAsync(i => i.Reference == reference, ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Invoice>> GetByPatientAsync(
        Guid patientId,
        InvoiceStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _db.Invoices.AsNoTracking().Where(i => i.PatientId == patientId);

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        return await query.OrderByDescending(i => i.IssuedAt).ToListAsync(ct);
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
}
