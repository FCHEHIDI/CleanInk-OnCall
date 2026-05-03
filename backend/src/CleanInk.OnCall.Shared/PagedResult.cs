namespace CleanInk.OnCall.Shared;

/// <summary>
/// Paginated result wrapper for query responses.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
/// <param name="Items">The items on the current page.</param>
/// <param name="Page">Current page number (1-based).</param>
/// <param name="PageSize">Number of items per page.</param>
/// <param name="TotalCount">Total number of items across all pages.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Creates a <see cref="PagedResult{T}"/> from a full list by slicing in memory.
    /// Prefer database-level pagination for production use.
    /// </summary>
    /// <param name="source">The full list of items.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <returns>A new <see cref="PagedResult{T}"/>.</returns>
    public static PagedResult<T> From(IReadOnlyList<T> source, int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be >= 1.");

        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(items, page, pageSize, source.Count);
    }
}
