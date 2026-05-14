using FluxOperations.Domain.Enums;

namespace FluxOperations.Application.DTOs;

public record AlertDto(
    Guid Id,
    Guid FluxId,
    string? FluxName,
    AlertSeverity Severity,
    string Message,
    bool IsResolved,
    DateTime? ResolvedAt,
    string? ResolvedBy,
    string? ResolutionNote,
    DateTime CreatedAt
);
