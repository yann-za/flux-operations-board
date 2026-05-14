using FluxOperations.Domain.Enums;

namespace FluxOperations.Application.DTOs;

public record FluxDto(
    Guid Id,
    string Name,
    string? Description,
    FluxType Type,
    FluxStatus Status,
    string? SourceSystem,
    string? TargetSystem,
    string? ScheduleCron,
    DateTime? LastExecutedAt,
    DateTime? NextExecutionAt,
    long? ThroughputPerHour,
    double? ErrorRatePercent,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
