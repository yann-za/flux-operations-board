using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Application.Common.Mappings;
using FluxOperations.Application.DTOs;
using FluxOperations.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Commands.Alerts;

public sealed class CreateAlertCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateAlertCommand, AlertDto>
{
    public async Task<AlertDto> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
    {
        var fluxExists = await context.Fluxes
            .AnyAsync(f => f.Id == request.FluxId, cancellationToken);

        if (!fluxExists)
            throw new KeyNotFoundException($"Flux {request.FluxId} not found.");

        var alert = Alert.Create(request.FluxId, request.Severity, request.Message);
        context.Alerts.Add(alert);
        await context.SaveChangesAsync(cancellationToken);

        return alert.ToDto(null);
    }
}
