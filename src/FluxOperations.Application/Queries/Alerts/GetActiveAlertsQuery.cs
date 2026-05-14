using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Enums;
using MediatR;

namespace FluxOperations.Application.Queries.Alerts;

public record GetActiveAlertsQuery(
    Guid? FluxId = null,
    AlertSeverity? Severity = null
) : IRequest<IReadOnlyList<AlertDto>>;
