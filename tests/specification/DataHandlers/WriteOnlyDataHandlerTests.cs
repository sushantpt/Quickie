using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using specification.Helpers.DataHandlers;
using specification.Helpers.Entities;

namespace specification.DataHandlers;

/// <summary>
/// Tests for writeOnly data handler 
/// </summary>
public class WriteOnlyDataHandlerTests
{
    private readonly Mock<DbSet<WriteOnlyEntityConcrete>> _mockDbSet;
    private readonly Mock<DbContext> _mockDbContext;
    private readonly WriteOnlyDataHandlerConcrete _handler;

    public WriteOnlyDataHandlerTests()
    {
        var mockOptions = new QuickieOptions { ShowCustomErrorMessage = true };
        GlobalQuickieConfigData.Initialize(mockOptions);
        
        _mockDbSet = new Mock<DbSet<WriteOnlyEntityConcrete>>();
        _mockDbContext = new Mock<DbContext>();

        _mockDbContext.Setup(x => x.Set<WriteOnlyEntityConcrete>()).Returns(_mockDbSet.Object);

        _handler = new WriteOnlyDataHandlerConcrete(_mockDbContext.Object);
    }

    /// <summary>
    /// Test CreateItemAsync when item is created successfully
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ShouldCreateItem_WhenItemIsValid()
    {
        // Arrange
        var item = new WriteOnlyEntityConcrete();

        _mockDbSet.Setup(x => x.AddAsync(item, CancellationToken.None))
                  .ReturnsAsync((EntityEntry<WriteOnlyEntityConcrete>)null!);

        _mockDbContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _handler.CreateItemAsync(item);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Item created successfully.", result.Message);
        Assert.Equal(item, result.Data);
    }

    /// <summary>
    /// Test CreateItemAsync when an exception occurs
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ShouldReturnError_WhenExceptionOccurs()
    {
        // Arrange
        var item = new WriteOnlyEntityConcrete();

        _mockDbSet.Setup(x => x.AddAsync(item, CancellationToken.None)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.CreateItemAsync(item);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("An error occured while creating the data", result.Message);
        Assert.Equal(item, result.Data);
    }

    
    /// <summary>
    /// Test CreateItemsAsync when multiple items are created successfully
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ShouldCreateItems_WhenItemsAreValid()
    {
        // arrange
        var items = new List<WriteOnlyEntityConcrete>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" }
        };

        var mockTransaction = new Mock<IDbContextTransaction>();
        mockTransaction.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var mockDatabaseFacade = new Mock<DatabaseFacade>(MockBehavior.Loose, new object[] { new Mock<DbContext>().Object });
        mockDatabaseFacade.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mockTransaction.Object);

        var mockDbSet = new Mock<DbSet<WriteOnlyEntityConcrete>>();
        mockDbSet.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<WriteOnlyEntityConcrete>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var mockDbContext = new Mock<DbContext>();
        mockDbContext.Setup(x => x.Set<WriteOnlyEntityConcrete>()).Returns(mockDbSet.Object);
        mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items.Count);
        mockDbContext.Setup(x => x.Database).Returns(mockDatabaseFacade.Object);

        var handler = new WriteOnlyDataHandlerConcrete(mockDbContext.Object);

        // Act
        var result = await handler.CreateItemsAsync(items);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Items created successfully.", result.Message);
        Assert.Equal(items, result.Data);

        // verify
        mockDbSet.Verify(x => x.AddRangeAsync(items, It.IsAny<CancellationToken>()), Times.Once);
        mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDatabaseFacade.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    /// <summary>
    /// Test CreateItemsAsync when an exception occurs. 
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ShouldReturnError_WhenExceptionOccurs()
    {
        // Arrange
        var items = new List<WriteOnlyEntityConcrete> { new(), new() };

        _mockDbSet.Setup(x => x.AddRangeAsync(items, CancellationToken.None)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.CreateItemsAsync(items);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("An error occured while creating the data", result.Message);
        Assert.Equal(items, result.Data);
    }

    /// <summary>
    /// Test when passing null item to CreateItemAsync
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ShouldThrowArgumentNullException_WhenItemIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.CreateItemAsync(null!));
    }

    /// <summary>
    /// Test when passing null items to CreateItemsAsync
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ShouldThrowArgumentNullException_WhenItemsAreNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.CreateItemsAsync(null!));
    }
}