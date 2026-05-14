using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Enums;
using MediatR;

namespace FluxOperations.Application.Commands.Fluxes;

public record CreateFluxCommand(
    string Name,
    FluxType Type,
    string? Description,
    string? SourceSystem,
    string? TargetSystem,
    string? ScheduleCron
) : IRequest<FluxDto>;
