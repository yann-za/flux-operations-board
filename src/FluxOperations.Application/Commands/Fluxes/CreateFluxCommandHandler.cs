using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.Common.Mappings;
using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Entities;
using MediatR;

namespace FluxOperations.Application.Commands.Fluxes;

public sealed class CreateFluxCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateFluxCommand, FluxDto>
{
    public async Task<FluxDto> Handle(CreateFluxCommand request, CancellationToken cancellationToken)
    {
        var flux = Flux.Create(
            request.Name,
            request.Type,
            request.Description,
            request.SourceSystem,
            request.TargetSystem,
            request.ScheduleCron);

        context.Fluxes.Add(flux);
        await context.SaveChangesAsync(cancellationToken);

        return flux.ToDto();
    }
}
