using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Enums;
using MediatR;

namespace FluxOperations.Application.Commands.Alerts;

public record CreateAlertCommand(
    Guid FluxId,
    AlertSeverity Severity,
    string Message
) : IRequest<AlertDto>;
