using FluentAssertions;
using FluxOperations.Application.Commands.Alerts;
using FluxOperations.Application.Tests.Common;
using FluxOperations.Domain.Entities;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Application.Tests.Commands;

public class ResolveAlertCommandHandlerTests
{
    [Fact]
    public async Task Handle_ActiveAlert_ResolvesSuccessfully()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var flux = context.Fluxes.First();

        var alert = Alert.Create(flux.Id, AlertSeverity.Critical, "Test critical alert");
        context.Alerts.Add(alert);
        await context.SaveChangesAsync();

        var handler = new ResolveAlertCommandHandler(context);
        var command = new ResolveAlertCommand(alert.Id, "operator@fluxboard.io", "Issue fixed in config.");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var resolved = context.Alerts.First(a => a.Id == alert.Id);
        resolved.IsResolved.Should().BeTrue();
        resolved.ResolvedBy.Should().Be("operator@fluxboard.io");
        resolved.ResolutionNote.Should().Be("Issue fixed in config.");
        resolved.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_AlreadyResolvedAlert_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var flux = context.Fluxes.First();

        var alert = Alert.Create(flux.Id, AlertSeverity.Warning, "Test warning");
        alert.Resolve("first.resolver");
        context.Alerts.Add(alert);
        await context.SaveChangesAsync();

        var handler = new ResolveAlertCommandHandler(context);

        // Act
        var act = async () => await handler.Handle(
            new ResolveAlertCommand(alert.Id, "second.resolver", null),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already resolved*");
    }

    [Fact]
    public async Task Handle_NonExistentAlert_ThrowsKeyNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new ResolveAlertCommandHandler(context);

        // Act
        var act = async () => await handler.Handle(
            new ResolveAlertCommand(Guid.NewGuid(), "user", null),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
