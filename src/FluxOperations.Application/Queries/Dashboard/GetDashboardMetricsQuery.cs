using FluxOperations.Application.DTOs;
using MediatR;

namespace FluxOperations.Application.Queries.Dashboard;

public record GetDashboardMetricsQuery : IRequest<DashboardMetricsDto>;
