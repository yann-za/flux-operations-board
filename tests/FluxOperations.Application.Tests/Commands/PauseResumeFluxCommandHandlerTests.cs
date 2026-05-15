using FluentAssertions;
using FluxOperations.Application.Commands.Fluxes;
using FluxOperations.Application.Tests.Common;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Application.Tests.Commands;

public class PauseResumeFluxCommandHandlerTests
{
    [Fact]
    public async Task PauseHandler_ActiveFlux_SetsStatusToPaused()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var activeFlux = context.Fluxes.First(f => f.Status == FluxStatus.Active);
        var handler = new PauseFluxCommandHandler(context);

        // Act
        await handler.Handle(new PauseFluxCommand(activeFlux.Id), CancellationToken.None);

        // Assert
        var updated = context.Fluxes.First(f => f.Id == activeFlux.Id);
        updated.Status.Should().Be(FluxStatus.Paused);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task PauseHandler_InactiveFlux_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var inactiveFlux = context.Fluxes.First(f => f.Status == FluxStatus.Inactive);
        var handler = new PauseFluxCommandHandler(context);

        // Act
        var act = async () => await handler.Handle(new PauseFluxCommand(inactiveFlux.Id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot pause*");
    }

    [Fact]
    public async Task ResumeHandler_PausedFlux_SetsStatusToActive()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var activeFlux = context.Fluxes.First(f => f.Status == FluxStatus.Active);

        var pauseHandler = new PauseFluxCommandHandler(context);
        await pauseHandler.Handle(new PauseFluxCommand(activeFlux.Id), CancellationToken.None);

        var resumeHandler = new ResumeFluxCommandHandler(context);

        // Act
        await resumeHandler.Handle(new ResumeFluxCommand(activeFlux.Id), CancellationToken.None);

        // Assert
        var updated = context.Fluxes.First(f => f.Id == activeFlux.Id);
        updated.Status.Should().Be(FluxStatus.Active);
    }

    [Fact]
    public async Task PauseHandler_NonExistentFlux_ThrowsKeyNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new PauseFluxCommandHandler(context);

        // Act
        var act = async () => await handler.Handle(new PauseFluxCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
