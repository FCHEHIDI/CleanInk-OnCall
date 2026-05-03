using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="Invoice"/> aggregate persistence.
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>Retrieves an invoice by its unique identifier.</summary>
    /// <param name="id">The invoice identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The invoice, or <c>null</c> if not found.</returns>
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Retrieves a paginated list of invoices, optionally filtered by customer.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="customerId">Optional customer filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="PagedResult{Invoice}"/> for the requested page.</returns>
    Task<PagedResult<Invoice>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? customerId = null,
        CancellationToken ct = default);

    /// <summary>Adds a new invoice to the repository.</summary>
    /// <param name="invoice">The invoice aggregate to persist.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(Invoice invoice, CancellationToken ct = default);

    /// <summary>Updates an existing invoice in the repository.</summary>
    /// <param name="invoice">The invoice aggregate to update.</param>
    void Update(Invoice invoice);

    /// <summary>Persists all pending changes.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
