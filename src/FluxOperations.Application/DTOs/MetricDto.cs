namespace FluxOperations.Application.DTOs;

public record MetricDto(
    Guid Id,
    Guid FluxId,
    string Name,
    double Value,
    string Unit,
    DateTime RecordedAt
);
