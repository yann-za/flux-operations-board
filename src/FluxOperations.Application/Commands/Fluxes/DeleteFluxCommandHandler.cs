using FluxOperations.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Commands.Fluxes;

public sealed class DeleteFluxCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteFluxCommand>
{
    public async Task Handle(DeleteFluxCommand request, CancellationToken cancellationToken)
    {
        var flux = await context.Fluxes
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Flux {request.Id} not found.");

        flux.Archive();
        await context.SaveChangesAsync(cancellationToken);
    }
}
