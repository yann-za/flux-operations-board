using FluxOperations.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Commands.Fluxes;

public sealed class PauseFluxCommandHandler(IAppDbContext context)
    : IRequestHandler<PauseFluxCommand>
{
    public async Task Handle(PauseFluxCommand request, CancellationToken cancellationToken)
    {
        var flux = await context.Fluxes
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Flux {request.Id} not found.");

        flux.Pause();
        await context.SaveChangesAsync(cancellationToken);
    }
}
