using FluentAssertions;
using FluxOperations.Domain.Entities;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Domain.Tests.Entities;

public class AlertEntityTests
{
    [Fact]
    public void Create_ValidParameters_ReturnsUnresolvedAlert()
    {
        // Arrange
        var fluxId = Guid.NewGuid();

        // Act
        var alert = Alert.Create(fluxId, AlertSeverity.Critical, "Critical error detected");

        // Assert
        alert.Should().NotBeNull();
        alert.Id.Should().NotBeEmpty();
        alert.FluxId.Should().Be(fluxId);
        alert.Severity.Should().Be(AlertSeverity.Critical);
        alert.Message.Should().Be("Critical error detected");
        alert.IsResolved.Should().BeFalse();
        alert.ResolvedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_EmptyMessage_ThrowsArgumentException(string? message)
    {
        // Act
        var act = () => Alert.Create(Guid.NewGuid(), AlertSeverity.Info, message!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Resolve_UnresolvedAlert_SetsResolvedFields()
    {
        // Arrange
        var alert = Alert.Create(Guid.NewGuid(), AlertSeverity.Warning, "Warning detected");
        var resolvedBy = "operator@fluxboard.io";
        var note = "Fixed by increasing timeout";
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        alert.Resolve(resolvedBy, note);

        // Assert
        alert.IsResolved.Should().BeTrue();
        alert.ResolvedBy.Should().Be(resolvedBy);
        alert.ResolutionNote.Should().Be(note);
        alert.ResolvedAt.Should().NotBeNull();
        alert.ResolvedAt.Should().BeAfter(before);
    }

    [Fact]
    public void Resolve_AlreadyResolvedAlert_ThrowsInvalidOperationException()
    {
        // Arrange
        var alert = Alert.Create(Guid.NewGuid(), AlertSeverity.Info, "Info message");
        alert.Resolve("first.user");

        // Act
        var act = () => alert.Resolve("second.user");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already resolved*");
    }

    [Fact]
    public void Resolve_WithoutNote_SetsNullResolutionNote()
    {
        // Arrange
        var alert = Alert.Create(Guid.NewGuid(), AlertSeverity.Warning, "Test");

        // Act
        alert.Resolve("admin");

        // Assert
        alert.IsResolved.Should().BeTrue();
        alert.ResolutionNote.Should().BeNull();
    }
}
