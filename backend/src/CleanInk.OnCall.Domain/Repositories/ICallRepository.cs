using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="Call"/> aggregate persistence.
/// Implementations live in the Infrastructure layer.
/// </summary>
public interface ICallRepository
{
    /// <summary>Retrieves a call by its unique identifier.</summary>
    /// <param name="id">The call identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The call, or <c>null</c> if not found.</returns>
    Task<Call?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Retrieves a paginated list of calls, optionally filtered by customer or operator.</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="customerId">Optional customer filter.</param>
    /// <param name="operatorId">Optional operator filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="PagedResult{Call}"/> for the requested page.</returns>
    Task<PagedResult<Call>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? customerId = null,
        Guid? operatorId = null,
        CancellationToken ct = default);

    /// <summary>Adds a new call to the repository.</summary>
    /// <param name="call">The call aggregate to persist.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(Call call, CancellationToken ct = default);

    /// <summary>Updates an existing call in the repository.</summary>
    /// <param name="call">The call aggregate to update.</param>
    void Update(Call call);

    /// <summary>Persists all pending changes.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
