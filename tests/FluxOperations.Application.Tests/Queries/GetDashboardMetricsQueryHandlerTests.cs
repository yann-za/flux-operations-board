using FluentAssertions;
using FluxOperations.Application.Queries.Dashboard;
using FluxOperations.Application.Tests.Common;
using FluxOperations.Domain.Entities;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Application.Tests.Queries;

public class GetDashboardMetricsQueryHandlerTests
{
    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsZeroMetrics()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new GetDashboardMetricsQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetDashboardMetricsQuery(), CancellationToken.None);

        // Assert
        result.TotalFluxes.Should().Be(0);
        result.ActiveFluxes.Should().Be(0);
        result.ActiveAlerts.Should().Be(0);
        result.AverageErrorRate.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithFluxesAndAlerts_ReturnsCorrectCounts()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var flux1 = FluxOperations.Domain.Entities.Flux.Create("Flux Active 1", FluxType.ETL);
        flux1.Activate();
        flux1.RecordExecution(1000, 2.0);

        var flux2 = FluxOperations.Domain.Entities.Flux.Create("Flux Active 2", FluxType.Streaming);
        flux2.Activate();
        flux2.RecordExecution(5000, 7.0);

        var flux3 = FluxOperations.Domain.Entities.Flux.Create("Flux Inactive", FluxType.BatchProcessing);

        context.Fluxes.AddRange(flux1, flux2, flux3);
        await context.SaveChangesAsync();

        var criticalAlert = Alert.Create(flux1.Id, AlertSeverity.Critical, "Critical issue");
        var warningAlert = Alert.Create(flux2.Id, AlertSeverity.Warning, "Warning issue");
        context.Alerts.AddRange(criticalAlert, warningAlert);
        await context.SaveChangesAsync();

        var handler = new GetDashboardMetricsQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetDashboardMetricsQuery(), CancellationToken.None);

        // Assert
        result.TotalFluxes.Should().Be(3);
        result.ActiveFluxes.Should().Be(1);
        result.FluxesInWarning.Should().Be(1);
        result.ActiveAlerts.Should().Be(2);
        result.CriticalAlerts.Should().Be(1);
        result.TotalThroughputPerHour.Should().Be(6000);
        result.AverageErrorRate.Should().BeApproximately(3.0, 0.1);
    }

    [Fact]
    public async Task Handle_StatusBreakdown_ContainsAllPresentStatuses()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var handler = new GetDashboardMetricsQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetDashboardMetricsQuery(), CancellationToken.None);

        // Assert
        result.FluxStatusBreakdown.Should().NotBeEmpty();
        result.FluxStatusBreakdown.Should().AllSatisfy(s =>
        {
            s.Status.Should().NotBeNullOrEmpty();
            s.Color.Should().StartWith("#");
            s.Count.Should().BeGreaterThan(0);
        });
    }
}
