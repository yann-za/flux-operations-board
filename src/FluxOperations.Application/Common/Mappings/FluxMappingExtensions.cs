using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Entities;

namespace FluxOperations.Application.Common.Mappings;

public static class FluxMappingExtensions
{
    public static FluxDto ToDto(this Flux flux) => new(
        flux.Id,
        flux.Name,
        flux.Description,
        flux.Type,
        flux.Status,
        flux.SourceSystem,
        flux.TargetSystem,
        flux.ScheduleCron,
        flux.LastExecutedAt,
        flux.NextExecutionAt,
        flux.ThroughputPerHour,
        flux.ErrorRatePercent,
        flux.IsArchived,
        flux.CreatedAt,
        flux.UpdatedAt
    );

    public static AlertDto ToDto(this Alert alert, string? fluxName) => new(
        alert.Id,
        alert.FluxId,
        fluxName,
        alert.Severity,
        alert.Message,
        alert.IsResolved,
        alert.ResolvedAt,
        alert.ResolvedBy,
        alert.ResolutionNote,
        alert.CreatedAt
    );

    public static MetricDto ToDto(this Metric metric) => new(
        metric.Id,
        metric.FluxId,
        metric.Name,
        metric.Value,
        metric.Unit,
        metric.RecordedAt
    );
}
