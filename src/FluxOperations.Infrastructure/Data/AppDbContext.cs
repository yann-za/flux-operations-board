using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Domain.Common;
using FluxOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Flux> Fluxes => Set<Flux>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Metric> Metrics => Set<Metric>();
    public DbSet<Dashboard> Dashboards => Set<Dashboard>();
    public DbSet<Widget> Widgets => Set<Widget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DispatchDomainEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void DispatchDomainEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
            entity.ClearDomainEvents();
    }
}
