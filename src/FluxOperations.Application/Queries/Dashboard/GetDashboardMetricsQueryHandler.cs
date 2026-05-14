using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Queries.Dashboard;

public sealed class GetDashboardMetricsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var fluxes = await context.Fluxes
            .AsNoTracking()
            .Where(f => !f.IsArchived)
            .ToListAsync(cancellationToken);

        var alerts = await context.Alerts
            .AsNoTracking()
            .Include(a => a.Flux)
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.Severity)
            .ThenByDescending(a => a.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var statusBreakdown = fluxes
            .GroupBy(f => f.Status)
            .Select(g => new FluxStatusSummary(
                g.Key.ToString(),
                g.Count(),
                GetStatusColor(g.Key)))
            .ToList();

        var recentAlerts = alerts.Select(a => new RecentAlertDto(
            a.Id,
            a.FluxId,
            a.Flux?.Name ?? "Unknown",
            a.Severity.ToString(),
            a.Message,
            a.CreatedAt)).ToList();

        return new DashboardMetricsDto(
            TotalFluxes: fluxes.Count,
            ActiveFluxes: fluxes.Count(f => f.Status == FluxStatus.Active),
            FluxesInError: fluxes.Count(f => f.Status == FluxStatus.Error),
            FluxesInWarning: fluxes.Count(f => f.Status == FluxStatus.Warning),
            ActiveAlerts: alerts.Count,
            CriticalAlerts: alerts.Count(a => a.Severity == AlertSeverity.Critical),
            TotalThroughputPerHour: fluxes.Sum(f => f.ThroughputPerHour ?? 0),
            AverageErrorRate: fluxes.Any() ? fluxes.Average(f => f.ErrorRatePercent ?? 0) : 0,
            FluxStatusBreakdown: statusBreakdown,
            RecentAlerts: recentAlerts
        );
    }

    private static string GetStatusColor(FluxStatus status) => status switch
    {
        FluxStatus.Active => "#22c55e",
        FluxStatus.Warning => "#f59e0b",
        FluxStatus.Error => "#ef4444",
        FluxStatus.Paused => "#6b7280",
        FluxStatus.Inactive => "#94a3b8",
        FluxStatus.Completed => "#3b82f6",
        _ => "#94a3b8"
    };
}
