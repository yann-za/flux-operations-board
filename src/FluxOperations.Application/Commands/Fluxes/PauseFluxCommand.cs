using MediatR;

namespace FluxOperations.Application.Commands.Fluxes;

public record PauseFluxCommand(Guid Id) : IRequest;
