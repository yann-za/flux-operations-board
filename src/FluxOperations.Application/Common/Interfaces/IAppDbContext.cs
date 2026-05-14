using FluxOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Flux> Fluxes { get; }
    DbSet<Alert> Alerts { get; }
    DbSet<Metric> Metrics { get; }
    DbSet<Dashboard> Dashboards { get; }
    DbSet<Widget> Widgets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
