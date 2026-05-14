using FluxOperations.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Commands.Alerts;

public sealed class ResolveAlertCommandHandler(IAppDbContext context)
    : IRequestHandler<ResolveAlertCommand>
{
    public async Task Handle(ResolveAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await context.Alerts
            .FirstOrDefaultAsync(a => a.Id == request.AlertId, cancellationToken)
            ?? throw new KeyNotFoundException($"Alert {request.AlertId} not found.");

        alert.Resolve(request.ResolvedBy, request.ResolutionNote);
        await context.SaveChangesAsync(cancellationToken);
    }
}
