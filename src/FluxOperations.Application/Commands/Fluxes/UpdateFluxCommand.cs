using FluxOperations.Application.DTOs;
using MediatR;

namespace FluxOperations.Application.Commands.Fluxes;

public record UpdateFluxCommand(
    Guid Id,
    string Name,
    string? Description,
    string? SourceSystem,
    string? TargetSystem,
    string? ScheduleCron
) : IRequest<FluxDto>;
