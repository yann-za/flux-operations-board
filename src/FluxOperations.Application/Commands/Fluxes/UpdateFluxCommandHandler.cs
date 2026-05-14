using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.Common.Mappings;
using FluxOperations.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Commands.Fluxes;

public sealed class UpdateFluxCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateFluxCommand, FluxDto>
{
    public async Task<FluxDto> Handle(UpdateFluxCommand request, CancellationToken cancellationToken)
    {
        var flux = await context.Fluxes
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Flux {request.Id} not found.");

        flux.Update(request.Name, request.Description, request.SourceSystem, request.TargetSystem, request.ScheduleCron);
        await context.SaveChangesAsync(cancellationToken);

        return flux.ToDto();
    }
}
