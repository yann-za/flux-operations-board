using FluxOperations.Application.Common.Models;
using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Enums;
using MediatR;

namespace FluxOperations.Application.Queries.Fluxes;

public record GetAllFluxesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    FluxStatus? StatusFilter = null,
    FluxType? TypeFilter = null,
    string? SearchTerm = null,
    bool IncludeArchived = false
) : IRequest<PaginatedList<FluxDto>>;
