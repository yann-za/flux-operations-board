using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.Common.Mappings;
using FluxOperations.Application.Common.Models;
using FluxOperations.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Queries.Fluxes;

public sealed class GetAllFluxesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAllFluxesQuery, PaginatedList<FluxDto>>
{
    public async Task<PaginatedList<FluxDto>> Handle(GetAllFluxesQuery request, CancellationToken cancellationToken)
    {
        var query = context.Fluxes.AsNoTracking();

        if (!request.IncludeArchived)
            query = query.Where(f => !f.IsArchived);

        if (request.StatusFilter.HasValue)
            query = query.Where(f => f.Status == request.StatusFilter.Value);

        if (request.TypeFilter.HasValue)
            query = query.Where(f => f.Type == request.TypeFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(f =>
                f.Name.Contains(request.SearchTerm) ||
                (f.Description != null && f.Description.Contains(request.SearchTerm)) ||
                (f.SourceSystem != null && f.SourceSystem.Contains(request.SearchTerm)));

        var projected = query
            .OrderByDescending(f => f.UpdatedAt ?? f.CreatedAt)
            .Select(f => f.ToDto());

        return await PaginatedList<FluxDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
