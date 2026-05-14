using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.Common.Mappings;
using FluxOperations.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Queries.Alerts;

public sealed class GetActiveAlertsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetActiveAlertsQuery, IReadOnlyList<AlertDto>>
{
    public async Task<IReadOnlyList<AlertDto>> Handle(GetActiveAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Alerts
            .AsNoTracking()
            .Include(a => a.Flux)
            .Where(a => !a.IsResolved);

        if (request.FluxId.HasValue)
            query = query.Where(a => a.FluxId == request.FluxId.Value);

        if (request.Severity.HasValue)
            query = query.Where(a => a.Severity == request.Severity.Value);

        var alerts = await query
            .OrderByDescending(a => a.Severity)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return alerts.Select(a => a.ToDto(a.Flux?.Name)).ToList();
    }
}
