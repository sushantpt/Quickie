using Microsoft.EntityFrameworkCore;
using Moq;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.DataObj;
using Quickie.Types.Readonly;
using specification.Helpers.DataHandlers;
using specification.Helpers.Entities;
using specification.Helpers.TestsDbContext;

namespace specification.DataHandlers;

/// <summary>
/// Test for readonly collection data handler. (integration tests with InMemory database.)
/// </summary>
public class ReadOnlyCollectionDataHandlerTests
{
    public ReadOnlyCollectionDataHandlerTests()
    {
        var mockOptions = new QuickieOptions { ShowCustomErrorMessage = true };
        GlobalQuickieConfigData.Initialize(mockOptions);
        
        Mock<DbSet<ReadOnlyEntity>> mockDbSet = new();
        Mock<DbContext> mockDbContext = new();

        mockDbContext.Setup(x => x.Set<ReadOnlyEntity>()).Returns(mockDbSet.Object);
    }

    /// <summary>
    /// GetByFilter returns proper paginated data.
    /// </summary>
    [Fact]
    public async Task GetByFilterAsync_WithValidFilter_ReturnsCorrectPaginatedData()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        await SeedReadOnlyEntities();
            
        // act
        var handler = new ReadOnlyCollectionDataHandlerConcrete(context);
        var request = new RequestForDataObj { PageNumber = 1, PageSize = 2 };
        var result = await handler.GetByFilterAsync(default, request);

        // assert
        Assert.NotNull(result.Items);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(5, result.Total);
    }

    /// <summary>
    /// GetByFilter returns correct page data.
    /// </summary>
    [Fact]
    public async Task GetByFilterAsync_WithPagination_ReturnsCorrectPage()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        await SeedReadOnlyEntities();
            
        // act
        var handler = new ReadOnlyCollectionDataHandlerConcrete(context);
        var request = new RequestForDataObj { PageNumber = 3, PageSize = 2 }; // (3-1=2) * 2 = 4, so 5 - 4 = 1 in items
        var result = await handler.GetByFilterAsync(default, request);

        // assert
        Assert.NotNull(result.Items);
        Assert.Single(result.Items);
        Assert.Equal(5, result.Total);
    }

    /// <summary>
    /// GetByFilter returns proper paginated data.
    /// </summary>
    [Fact]
    public async Task GetByFilterAsync_WithCancellationToken_ShouldRespectToken()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabaseCancellation").Options;
        await using var context = new TestDbContext(dbOptions);

        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var cancelledToken = cancellationTokenSource.Token;

        // act & assert
        var handler = new ReadOnlyCollectionDataHandlerConcrete(context);
        var request = new RequestForDataObj { PageNumber = 1, PageSize = 10 };

        await Assert.ThrowsAsync<OperationCanceledException>(async () => await handler.GetByFilterAsync(default, request, cancellationToken: cancelledToken));
    }

    /// <summary>
    /// GetByFilter returns proper paginated data for predicated search.
    /// </summary>
    [Fact]
    public async Task GetByFilterAsync_WithPredicate_ReturnsCorrectPaginatedData()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        await SeedReadOnlyEntities();
            
        // act
        var handler = new ReadOnlyCollectionDataHandlerConcrete(context);
        var request = new RequestForDataObj { PageNumber = 1, PageSize = 5 };
        var result = await handler.GetByFilterAsync(x => x.Name.Contains("jon"), request);

        // assert
        Assert.NotNull(result.Items);
        Assert.Equal(2, result.Items.Count()); // db contains 2 names with jon
        Assert.Equal(2, result.Total); // 2 data with name jon
    }
    
    /// <summary>
    /// GetByFilter returns no data when predicate value does not exist.
    /// </summary>
    [Fact]
    public async Task GetByFilterAsync_WithPredicate_NoData()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        await SeedReadOnlyEntities();
            
        // act
        var handler = new ReadOnlyCollectionDataHandlerConcrete(context);
        var request = new RequestForDataObj { PageNumber = 1, PageSize = 5 };
        var result = await handler.GetByFilterAsync(x => x.Name.Contains("sushant"), request);

        // assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items); // db contains no data
        Assert.Equal(0, result.Total); // 0 data with following name
    }

    /// <summary>
    /// Seeding inMemory database.
    /// </summary>
    private static async Task SeedReadOnlyEntities()
    {
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        context.ReadOnlyEntityConcrete.RemoveRange(context.ReadOnlyEntityConcrete);
        await context.SaveChangesAsync();
        // seed with an entity
        var existingEntity = new List<ReadOnlyEntityConcrete>
        {
            new() { Id = 1, Name = "jon bones jones" },
            new() { Id = 2, Name = "alexandar pantoja" },
            new() { Id = 3, Name = "cryl gane" },
            new() { Id = 4, Name = "dana white" },
            new() { Id = 5, Name = "jon bernthal" },
        };
        await context.ReadOnlyEntityConcrete.AddRangeAsync(existingEntity);
        await context.SaveChangesAsync();
        await Task.CompletedTask;
    }
}