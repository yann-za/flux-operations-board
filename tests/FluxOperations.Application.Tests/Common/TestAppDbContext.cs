using FluxOperations.Application.Common.Interfaces;
using FluxOperations.Domain.Entities;
using FluxOperations.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Tests.Common;

public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static async Task<AppDbContext> CreateWithSeedAsync()
    {
        var context = Create();

        var flux1 = FluxOperations.Domain.Entities.Flux.Create(
            "Test ETL Pipeline", FluxOperations.Domain.Enums.FluxType.ETL, "Test description");
        flux1.Activate();

        var flux2 = FluxOperations.Domain.Entities.Flux.Create(
            "Test API Integration", FluxOperations.Domain.Enums.FluxType.ApiIntegration);

        context.Fluxes.Add(flux1);
        context.Fluxes.Add(flux2);
        await context.SaveChangesAsync();

        return context;
    }
}
