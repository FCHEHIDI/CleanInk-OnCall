namespace CleanInk.OnCall.Application.Dashboard.DTOs;

/// <summary>
/// Snapshot of live KPI metrics displayed on the dashboard.
/// All counts are scoped to the current tenant.
/// </summary>
/// <param name="CallsToday">Number of calls created today (UTC).</param>
/// <param name="OpenCalls">Number of calls in Pending or InProgress status.</param>
/// <param name="PendingInvoices">Number of invoices not yet paid (Draft or Issued).</param>
/// <param name="ActiveUsers">Number of currently active user accounts.</param>
public sealed record DashboardKpisDto(
    int CallsToday,
    int OpenCalls,
    int PendingInvoices,
    int ActiveUsers);
