using FluxOperations.Application.DTOs;
using MediatR;

namespace FluxOperations.Application.Queries.Fluxes;

public record GetFluxByIdQuery(Guid Id) : IRequest<FluxDto?>;
