using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using specification.Helpers.DataHandlers;
using specification.Helpers.Entities;
using specification.Helpers.TestsDbContext;

namespace specification.DataHandlers;

/// <summary>
/// Tests of CRUD data handler.
/// </summary>
public class CrudDataHandlerTests
{
    private readonly Mock<DbSet<CrudEntityConcrete>> _mockDbSet;
    private readonly Mock<DbContext> _mockDbContext;
    private readonly CrudDataHandlerConcrete _handler;

    public CrudDataHandlerTests()
    {
        _mockDbSet = new Mock<DbSet<CrudEntityConcrete>>();
        _mockDbContext = new Mock<DbContext>();
        var mockOptions = new QuickieOptions { ShowCustomErrorMessage = true };
        GlobalQuickieConfigData.Initialize(mockOptions);

        _mockDbContext.Setup(x => x.Set<CrudEntityConcrete>()).Returns(_mockDbSet.Object);
        _handler = new CrudDataHandlerConcrete(_mockDbContext.Object);
    }

    /// <summary>
    /// CreateAsync returns success with newly created data.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenEntityIsAdded()
    {
        var entity = new CrudEntityConcrete();
        _mockDbSet.Setup(m => m.AddAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<EntityEntry<CrudEntityConcrete>>());

        _mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        var result = await _handler.CreateAsync(entity, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Created.", result.Message);
        Assert.NotNull(result.Data);

        _mockDbSet.Verify(m => m.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// CreateAsync returns failure message when something went wrong.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenSaveFails()
    {
        var entity = new CrudEntityConcrete();
        _mockDbSet.Setup(m => m.AddAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<EntityEntry<CrudEntityConcrete>>());

        _mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var result = await _handler.CreateAsync(entity);

        Assert.False(result.IsSuccess);
        Assert.Equal("Data not created.", result.Message);
        Assert.NotNull(result.Data);
    }

    /// <summary>
    /// GetDataAsync returning data when it exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenEntityExists()
    {
        var entity = new CrudEntityConcrete();
        _mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(entity);

        var result = await _handler.GetByIdAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Equal("Data found.", result.Message);
        Assert.NotNull(result.Data);
    }

    /// <summary>
    /// GetDataAsync when data does not exist.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnFailure_WhenEntityDoesNotExist()
    {
        _mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((CrudEntityConcrete)null!);

        var result = await _handler.GetByIdAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Data not found.", result.Message);
        Assert.Null(result.Data);
    }
    
    /// <summary>
    /// Update success returns with updated data and message. (integration test)
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenEntityIsUpdated()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        // seed with an entity
        var existingEntity = new CrudEntityConcrete { Id = 1, Name = "Original Name" };
        await context.CrudEntityConcrete.AddAsync(existingEntity);
        await context.SaveChangesAsync();
        
        // create the updated entity
        var updatedEntity = new CrudEntityConcrete { Id = 1, Name = "Updated Name" };

        // act
        var handler = new CrudDataHandlerConcrete(context);
        var result = await handler.UpdateAsync(1, updatedEntity);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Entity updated successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated Name", result.Data.Name);

        // final verification
        var dbEntity = await context.CrudEntityConcrete.FindAsync(1);
        Assert.Equal("Updated Name", dbEntity!.Name);
    }

    /// <summary>
    /// Update failure returns with failure message.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldReturnFailure_WhenEntityDoesNotExist()
    {
        _mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((CrudEntityConcrete)null!);

        var result = await _handler.UpdateAsync(1, new CrudEntityConcrete());

        Assert.False(result.IsSuccess);
        Assert.Contains("Data with ID 1 not found", result.Message);
        Assert.Null(result.Data);
    }

    /// <summary>
    /// Successful deletion of data.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenEntityIsDeleted()
    {
        var entity = new CrudEntityConcrete();
        _mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(entity);
        _mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _handler.DeleteAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Equal("Data removed successfully.", result.Message);
    }

    /// <summary>
    /// When data to delete does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenEntityDoesNotExist()
    {
        _mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((CrudEntityConcrete)null!);

        var result = await _handler.DeleteAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Data not found.", result.Message);
    }
}