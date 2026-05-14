using MediatR;

namespace FluxOperations.Application.Commands.Fluxes;

public record DeleteFluxCommand(Guid Id) : IRequest;
