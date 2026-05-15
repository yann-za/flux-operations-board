using FluentAssertions;
using FluxOperations.Application.Commands.Fluxes;
using FluxOperations.Application.Tests.Common;
using FluxOperations.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FluxOperations.Application.Tests.Commands;

public class CreateFluxCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesFluxAndReturnsDto()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateFluxCommandHandler(context);
        var command = new CreateFluxCommand(
            Name: "SAP ETL Pipeline",
            Type: FluxType.ETL,
            Description: "Nightly extraction",
            SourceSystem: "SAP",
            TargetSystem: "DW",
            ScheduleCron: "0 2 * * *");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("SAP ETL Pipeline");
        result.Type.Should().Be(FluxType.ETL);
        result.Status.Should().Be(FluxStatus.Inactive);
        result.SourceSystem.Should().Be("SAP");
        result.IsArchived.Should().BeFalse();

        var storedFlux = await context.Fluxes.FirstOrDefaultAsync(f => f.Id == result.Id);
        storedFlux.Should().NotBeNull();
        storedFlux!.Name.Should().Be("SAP ETL Pipeline");
    }

    [Fact]
    public async Task Handle_MultipleCommands_CreatesDistinctFluxes()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateFluxCommandHandler(context);

        // Act
        var result1 = await handler.Handle(
            new CreateFluxCommand("Flux A", FluxType.ETL, null, null, null, null),
            CancellationToken.None);
        var result2 = await handler.Handle(
            new CreateFluxCommand("Flux B", FluxType.Streaming, null, null, null, null),
            CancellationToken.None);

        // Assert
        result1.Id.Should().NotBe(result2.Id);
        var count = await context.Fluxes.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task Handle_CreatedFlux_HasCorrectTimestamp()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateFluxCommandHandler(context);
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var result = await handler.Handle(
            new CreateFluxCommand("Time Test Flux", FluxType.BatchProcessing, null, null, null, null),
            CancellationToken.None);

        // Assert
        result.CreatedAt.Should().BeAfter(before);
        result.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        result.UpdatedAt.Should().BeNull();
    }
}
