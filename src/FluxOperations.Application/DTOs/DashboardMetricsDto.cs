namespace FluxOperations.Application.DTOs;

public record DashboardMetricsDto(
    int TotalFluxes,
    int ActiveFluxes,
    int FluxesInError,
    int FluxesInWarning,
    int ActiveAlerts,
    int CriticalAlerts,
    long TotalThroughputPerHour,
    double AverageErrorRate,
    IReadOnlyList<FluxStatusSummary> FluxStatusBreakdown,
    IReadOnlyList<RecentAlertDto> RecentAlerts
);

public record FluxStatusSummary(string Status, int Count, string Color);
public record RecentAlertDto(Guid AlertId, Guid FluxId, string FluxName, string Severity, string Message, DateTime OccurredAt);
