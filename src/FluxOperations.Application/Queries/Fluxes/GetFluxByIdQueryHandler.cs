using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.Common.Mappings;
using FluxOperations.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Queries.Fluxes;

public sealed class GetFluxByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetFluxByIdQuery, FluxDto?>
{
    public async Task<FluxDto?> Handle(GetFluxByIdQuery request, CancellationToken cancellationToken)
    {
        var flux = await context.Fluxes
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        return flux?.ToDto();
    }
}
