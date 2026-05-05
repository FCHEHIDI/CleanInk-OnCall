using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Application.Dashboard.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Dashboard.Queries;

/// <summary>
/// Query that returns the live KPI snapshot for the dashboard.
/// No parameters — metrics are scoped to the current tenant via <see cref="IDashboardReadModel"/>.
/// </summary>
public sealed record GetDashboardKpisQuery : IRequest<DashboardKpisDto>;

/// <summary>
/// Handles <see cref="GetDashboardKpisQuery"/> — delegates to the Infrastructure read-model
/// for optimised, cross-entity aggregation.
/// </summary>
public sealed class GetDashboardKpisQueryHandler : IRequestHandler<GetDashboardKpisQuery, DashboardKpisDto>
{
    private readonly IDashboardReadModel _readModel;
    private readonly ILogger<GetDashboardKpisQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GetDashboardKpisQueryHandler"/>.
    /// </summary>
    /// <param name="readModel">Dashboard read-model providing aggregated counts.</param>
    /// <param name="logger">Logger instance.</param>
    public GetDashboardKpisQueryHandler(
        IDashboardReadModel readModel,
        ILogger<GetDashboardKpisQueryHandler> logger)
    {
        _readModel = readModel;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DashboardKpisDto> Handle(
        GetDashboardKpisQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching dashboard KPIs");

        var kpis = await _readModel.GetKpisAsync(cancellationToken);

        _logger.LogInformation(
            "Dashboard KPIs — callsToday={CallsToday} openCalls={OpenCalls} pendingInvoices={PendingInvoices} activeUsers={ActiveUsers}",
            kpis.CallsToday, kpis.OpenCalls, kpis.PendingInvoices, kpis.ActiveUsers);

        return kpis;
    }
}
