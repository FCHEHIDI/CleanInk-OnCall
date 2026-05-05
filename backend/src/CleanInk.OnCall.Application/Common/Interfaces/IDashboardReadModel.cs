using CleanInk.OnCall.Application.Dashboard.DTOs;

namespace CleanInk.OnCall.Application.Common.Interfaces;

/// <summary>
/// Read-model contract for aggregated dashboard metrics.
/// Implemented in the Infrastructure layer to allow direct, optimised queries
/// without going through individual domain repositories.
/// </summary>
public interface IDashboardReadModel
{
    /// <summary>
    /// Returns live KPI counts for the current authenticated tenant.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="DashboardKpisDto"/> with the current metric snapshot.</returns>
    Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default);
}
