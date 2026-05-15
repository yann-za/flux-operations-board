using FluentAssertions;
using FluxOperations.Domain.Entities;
using FluxOperations.Domain.Enums;
using FluxOperations.Domain.Events;

namespace FluxOperations.Domain.Tests.Entities;

public class FluxEntityTests
{
    [Fact]
    public void Create_ValidParameters_ReturnsFluxInInactiveStatus()
    {
        // Act
        var flux = Flux.Create("Test Flux", FluxType.ETL, "Description", "Source", "Target", "0 2 * * *");

        // Assert
        flux.Should().NotBeNull();
        flux.Id.Should().NotBeEmpty();
        flux.Name.Should().Be("Test Flux");
        flux.Type.Should().Be(FluxType.ETL);
        flux.Status.Should().Be(FluxStatus.Inactive);
        flux.Description.Should().Be("Description");
        flux.SourceSystem.Should().Be("Source");
        flux.TargetSystem.Should().Be("Target");
        flux.IsArchived.Should().BeFalse();
    }

    [Fact]
    public void Create_ValidFlux_RaisesFluxCreatedDomainEvent()
    {
        // Act
        var flux = Flux.Create("Event Flux", FluxType.Streaming);

        // Assert
        flux.DomainEvents.Should().HaveCount(1);
        flux.DomainEvents.First().Should().BeOfType<FluxCreatedEvent>();
        var evt = (FluxCreatedEvent)flux.DomainEvents.First();
        evt.FluxId.Should().Be(flux.Id);
        evt.FluxName.Should().Be("Event Flux");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyOrNullName_ThrowsArgumentException(string? name)
    {
        // Act
        var act = () => Flux.Create(name!, FluxType.ETL);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Activate_InactiveFlux_SetsStatusToActive()
    {
        // Arrange
        var flux = Flux.Create("Activatable Flux", FluxType.ETL);

        // Act
        flux.Activate();

        // Assert
        flux.Status.Should().Be(FluxStatus.Active);
        flux.DomainEvents.Should().Contain(e => e is FluxStatusChangedEvent);
        var evt = flux.DomainEvents.OfType<FluxStatusChangedEvent>().Last();
        evt.NewStatus.Should().Be(FluxStatus.Active);
    }

    [Fact]
    public void Pause_ActiveFlux_SetsStatusToPaused()
    {
        // Arrange
        var flux = Flux.Create("Pauseable Flux", FluxType.BatchProcessing);
        flux.Activate();
        flux.ClearDomainEvents();

        // Act
        flux.Pause();

        // Assert
        flux.Status.Should().Be(FluxStatus.Paused);
        flux.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Pause_InactiveFlux_ThrowsInvalidOperationException()
    {
        // Arrange
        var flux = Flux.Create("Not Active Flux", FluxType.ETL);

        // Act
        var act = () => flux.Pause();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot pause*");
    }

    [Fact]
    public void Resume_PausedFlux_SetsStatusToActive()
    {
        // Arrange
        var flux = Flux.Create("Resumeable Flux", FluxType.ETL);
        flux.Activate();
        flux.Pause();

        // Act
        flux.Resume();

        // Assert
        flux.Status.Should().Be(FluxStatus.Active);
    }

    [Fact]
    public void Resume_NotPausedFlux_ThrowsInvalidOperationException()
    {
        // Arrange
        var flux = Flux.Create("Active Flux", FluxType.ETL);
        flux.Activate();

        // Act
        var act = () => flux.Resume();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot resume*");
    }

    [Fact]
    public void RecordExecution_HighErrorRate_SetsStatusToWarning()
    {
        // Arrange
        var flux = Flux.Create("Warning Flux", FluxType.ETL);
        flux.Activate();

        // Act
        flux.RecordExecution(1000, 8.5);

        // Assert
        flux.Status.Should().Be(FluxStatus.Warning);
        flux.ThroughputPerHour.Should().Be(1000);
        flux.ErrorRatePercent.Should().Be(8.5);
        flux.LastExecutedAt.Should().NotBeNull();
    }

    [Fact]
    public void RecordExecution_LowErrorRate_KeepsActiveStatus()
    {
        // Arrange
        var flux = Flux.Create("Healthy Flux", FluxType.ETL);
        flux.Activate();

        // Act
        flux.RecordExecution(5000, 0.5);

        // Assert
        flux.Status.Should().Be(FluxStatus.Active);
    }

    [Fact]
    public void MarkAsError_CreatesAlertWithCriticalSeverity()
    {
        // Arrange
        var flux = Flux.Create("Error Flux", FluxType.ETL);
        flux.Activate();

        // Act
        flux.MarkAsError("Connection timeout to SAP");

        // Assert
        flux.Status.Should().Be(FluxStatus.Error);
        flux.Alerts.Should().HaveCount(1);
        flux.Alerts.First().Severity.Should().Be(AlertSeverity.Critical);
        flux.Alerts.First().Message.Should().Contain("Connection timeout to SAP");
    }

    [Fact]
    public void Archive_SetsIsArchivedToTrue()
    {
        // Arrange
        var flux = Flux.Create("To Archive", FluxType.ETL);

        // Act
        flux.Archive();

        // Assert
        flux.IsArchived.Should().BeTrue();
    }
}
