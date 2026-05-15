using FluentAssertions;
using FluxOperations.Application.Queries.Fluxes;
using FluxOperations.Application.Tests.Common;
using FluxOperations.Domain.Enums;

namespace FluxOperations.Application.Tests.Queries;

public class GetAllFluxesQueryHandlerTests
{
    [Fact]
    public async Task Handle_NoFilters_ReturnsAllActiveFluxes()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var handler = new GetAllFluxesQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetAllFluxesQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.Should().AllSatisfy(f => f.IsArchived.Should().BeFalse());
    }

    [Fact]
    public async Task Handle_StatusFilter_ReturnsOnlyMatchingFluxes()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var handler = new GetAllFluxesQueryHandler(context);

        // Act
        var result = await handler.Handle(
            new GetAllFluxesQuery(StatusFilter: FluxStatus.Active),
            CancellationToken.None);

        // Assert
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(f => f.Status.Should().Be(FluxStatus.Active));
    }

    [Fact]
    public async Task Handle_SearchTerm_ReturnsMatchingFluxes()
    {
        // Arrange
        await using var context = await TestDbContextFactory.CreateWithSeedAsync();
        var handler = new GetAllFluxesQueryHandler(context);

        // Act
        var result = await handler.Handle(
            new GetAllFluxesQuery(SearchTerm: "ETL"),
            CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Contain("ETL");
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var handler = new GetAllFluxesQueryHandler(context);

        for (int i = 1; i <= 5; i++)
        {
            var f = FluxOperations.Domain.Entities.Flux.Create($"Flux {i}", FluxType.ETL);
            context.Fluxes.Add(f);
        }
        await context.SaveChangesAsync();

        // Act
        var page1 = await handler.Handle(new GetAllFluxesQuery(PageNumber: 1, PageSize: 2), CancellationToken.None);
        var page2 = await handler.Handle(new GetAllFluxesQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        // Assert
        page1.Items.Should().HaveCount(2);
        page2.Items.Should().HaveCount(2);
        page1.TotalCount.Should().Be(5);
        page1.TotalPages.Should().Be(3);
        page1.HasPreviousPage.Should().BeFalse();
        page1.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ArchivedFlux_ExcludedByDefault()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var flux = FluxOperations.Domain.Entities.Flux.Create("Archived Flux", FluxType.ETL);
        flux.Archive();
        context.Fluxes.Add(flux);
        await context.SaveChangesAsync();

        var handler = new GetAllFluxesQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetAllFluxesQuery(), CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
    }
}
