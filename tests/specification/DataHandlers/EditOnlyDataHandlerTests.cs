using Microsoft.EntityFrameworkCore;
using Moq;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.Types.Editonly;
using specification.Helpers.DataHandlers;
using specification.Helpers.Entities;
using specification.Helpers.TestsDbContext;

namespace specification.DataHandlers;

/// <summary>
/// Test for EditOnly data handler
/// </summary>
public class EditOnlyDataHandlerTests
{
    private readonly Mock<DbContext> _mockDbContext;
    private readonly EditOnlyDataHandlerConcrete _handler;

    public EditOnlyDataHandlerTests()
    {
        var mockOptions = new QuickieOptions { ShowCustomErrorMessage = true };
        GlobalQuickieConfigData.Initialize(mockOptions);
        
        Mock<DbSet<EditOnlyEntity>> mockDbSet = new();
        _mockDbContext = new Mock<DbContext>();

        _mockDbContext.Setup(x => x.Set<EditOnlyEntity>()).Returns(mockDbSet.Object);

        _handler = new EditOnlyDataHandlerConcrete(_mockDbContext.Object);
    }

    /// <summary>
    /// edit async success with message and updated data.
    /// </summary>
    [Fact]
    public async Task EditAsync_ShouldReturnSuccess_WhenEntityIsUpdated()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);
        // seed with an entity
        var existingEntity = new EditOnlyEntityConcrete { Id = 1, Name = "Original Name" };
        await context.EditOnlyEntityConcrete.AddAsync(existingEntity);
        await context.SaveChangesAsync();
        
        // create the updated entity
        var updatedEntity = new EditOnlyEntityConcrete { Id = 1, Name = "Updated Name" };

        // act
        var handler = new EditOnlyDataHandlerConcrete(context);
        var result = await handler.EditAsync(1, updatedEntity);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Entity updated successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated Name", result.Data.Name);
        Assert.Contains("Entity updated successfully", result.Message);

        // final verification
        var dbEntity = await context.EditOnlyEntityConcrete.FindAsync(1);
        Assert.Equal("Updated Name", dbEntity!.Name);
    }

    /// <summary>
    /// edit async failure with message when updating data does not exist.
    /// </summary>
    [Fact]
    public async Task EditAsync_ShouldReturnFailure_WhenEntityDoesNotExist()
    {
        // arrange
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDatabase").Options;
        await using var context = new TestDbContext(dbOptions);

        var nonExistentEntity = new EditOnlyEntityConcrete { Id = 1, Name = "Non-existent Entity" };

        // act
        var handler = new EditOnlyDataHandlerConcrete(context);
        var result = await handler.EditAsync(1, nonExistentEntity);

        // assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Data with ID 1 not found", result.Message);
        Assert.Null(result.Data);

        // final verification
        var dbEntity = await context.EditOnlyEntityConcrete.FindAsync(1);
        Assert.Null(dbEntity);
    }

    /// <summary>
    /// edit async failure with message when some error occurs (eg: updated data is not saved).
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_ShouldReturnFailure_WhenNoChangesAreMade()
    {
        var entities = new List<EditOnlyEntityConcrete> { new(), new() };

        _mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(0);

        var result = await _handler.EditCollectionAsync(entities);

        Assert.False(result.IsSuccess);
        Assert.Contains("An error occurred while updating the data", result.Message);
        Assert.Equal(entities, result.Data);
    }

    /// <summary>
    /// edit async failure with message when exception is thrown.
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        var entities = new List<EditOnlyEntityConcrete> { new(), new() };

        _mockDbContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(new Exception("An error occurred while updating the data"));

        var result = await _handler.EditCollectionAsync(entities);

        Assert.False(result.IsSuccess);
        Assert.Contains("An error occurred while updating the data", result.Message);
        Assert.Equal(entities, result.Data);
    }
    
    /// <summary>
    /// edit async failure with message when updating data (input) is null.
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_ShouldReturnError_WhenInputIsNull()
    {
        // Arrange
        var handler = new EditOnlyDataHandlerConcrete(_mockDbContext.Object);

        // Act 
        var result = await handler.EditCollectionAsync(null!);
        
        // assert
        Assert.False(result.IsSuccess);
        Assert.Contains("An error occurred while updating the data", result.Message);
    }
}