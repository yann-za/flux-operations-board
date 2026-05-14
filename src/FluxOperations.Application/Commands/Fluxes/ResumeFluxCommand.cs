using MediatR;

namespace FluxOperations.Application.Commands.Fluxes;

public record ResumeFluxCommand(Guid Id) : IRequest;
