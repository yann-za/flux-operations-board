using FluxOperations.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Commands.Fluxes;

public sealed class ResumeFluxCommandHandler(IAppDbContext context)
    : IRequestHandler<ResumeFluxCommand>
{
    public async Task Handle(ResumeFluxCommand request, CancellationToken cancellationToken)
    {
        var flux = await context.Fluxes
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Flux {request.Id} not found.");

        flux.Resume();
        await context.SaveChangesAsync(cancellationToken);
    }
}
