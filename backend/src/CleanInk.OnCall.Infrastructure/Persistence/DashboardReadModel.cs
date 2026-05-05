using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Application.Dashboard.DTOs;
using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanInk.OnCall.Infrastructure.Persistence;

/// <summary>
/// Infrastructure implementation of <see cref="IDashboardReadModel"/>.
/// Uses <see cref="AppDbContext"/> directly for efficient, cross-entity COUNT queries
/// without materialising full entity graphs.
/// All queries are automatically tenant-scoped through the schema resolver in <see cref="AppDbContext"/>.
/// </summary>
internal sealed class DashboardReadModel : IDashboardReadModel
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of <see cref="DashboardReadModel"/>.
    /// </summary>
    /// <param name="db">EF Core application database context.</param>
    public DashboardReadModel(AppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default)
    {
        var todayUtc = DateTime.UtcNow.Date;

        var callsToday = await _db.Calls
            .CountAsync(c => c.CreatedAt >= todayUtc, ct);

        var openCalls = await _db.Calls
            .CountAsync(c => c.Status == CallStatus.Pending || c.Status == CallStatus.InProgress, ct);

        var pendingInvoices = await _db.Invoices
            .CountAsync(i => i.Status == InvoiceStatus.Draft || i.Status == InvoiceStatus.Issued, ct);

        var activeUsers = await _db.Users
            .CountAsync(u => u.IsActive, ct);

        return new DashboardKpisDto(callsToday, openCalls, pendingInvoices, activeUsers);
    }
}
