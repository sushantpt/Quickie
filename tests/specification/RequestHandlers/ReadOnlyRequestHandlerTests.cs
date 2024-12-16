using Moq;
using Quickie.DataHandlers.Readonly.Definition;
using specification.Helpers.Entities;
using specification.Helpers.RequestHandlers;

namespace specification.RequestHandlers;

/// <summary>
/// Tests for read only request handler.
/// </summary>
public class ReadOnlyRequestHandlerTests
{
    private readonly ReadOnlyRequestHandlerConcrete _readOnlyRequestHandler;
    private readonly Mock<IReadOnlyDataHandler<ReadOnlyEntityConcrete, int>> _mockDataHandler;

    public ReadOnlyRequestHandlerTests()
    {
        _mockDataHandler = new Mock<IReadOnlyDataHandler<ReadOnlyEntityConcrete, int>>();
        _readOnlyRequestHandler = new ReadOnlyRequestHandlerConcrete(_mockDataHandler.Object);
    }
    
    /// <summary>
    /// GetByIdAsync returns data when exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenEntityExists()
    {
        var responseEntity = new ReadOnlyEntityConcrete();
        _mockDataHandler.Setup(m => m.GetByIdAsync(It.IsAny<int>(), default)).ReturnsAsync(responseEntity);
        
        var result = await _readOnlyRequestHandler.GetByIdAsync(1);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        _mockDataHandler.Verify(m => m.GetByIdAsync(1, default), Times.Once);
    }

    /// <summary>
    /// GetByIdAsync when data does not exist with proper message.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnFailure_WhenEntityDoesNotExist()
    {
        _mockDataHandler
            .Setup(m => m.GetByIdAsync(It.IsAny<int>(), default))
            .ReturnsAsync((ReadOnlyEntityConcrete)null!);

        var result = await _readOnlyRequestHandler.GetByIdAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Data not found.", result.Message);
        Assert.Null(result.Data);

        _mockDataHandler.Verify(m => m.GetByIdAsync(It.IsAny<int>(), default), Times.Once);
    }

    /// <summary>
    /// GetByIdAsync when id to search data is null with proper message.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentException_WhenIdIsNull()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _readOnlyRequestHandler.GetByIdAsync(0));
    }
}