using Moq;
using Quickie.DataHandlers.Writeonly.Definition;
using Quickie.DataObj;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;
using specification.Helpers.RequestHandlers;

namespace specification.RequestHandlers;

/// <summary>
/// Tests for write only request handler.
/// </summary>
public class WriteOnlyRequestHandlerTests
{
    private readonly Mock<IWriteOnlyDataHandler<WriteOnlyEntityConcrete>> _mockDataHandler;
    private readonly WriteOnlyRequestHandlerConcrete _handler;

    public WriteOnlyRequestHandlerTests()
    {
        _mockDataHandler = new Mock<IWriteOnlyDataHandler<WriteOnlyEntityConcrete>>();
        _handler = new WriteOnlyRequestHandlerConcrete(_mockDataHandler.Object);
    }

    /// <summary>
    /// When data is created, it returns success with newly created data. 
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ShouldReturnSuccessResponse_WhenItemIsCreatedSuccessfully()
    {
        var item = new WriteOnlyDtoConcrete();
        var entity = new WriteOnlyEntityConcrete();
        var dataHandlerResponse = new ResponseObj<WriteOnlyEntityConcrete>
        {
            IsSuccess = true,
            Data = entity
        };

        _mockDataHandler
            .Setup(m => m.CreateItemAsync(It.IsAny<WriteOnlyEntityConcrete>(), default))
            .ReturnsAsync(dataHandlerResponse);

        var result = await _handler.CreateItemAsync(item, default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        _mockDataHandler.Verify(m => m.CreateItemAsync(It.IsAny<WriteOnlyEntityConcrete>(), default), Times.Once);
    }

    /// <summary>
    /// When data is not created, it returns failure with message.
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ShouldReturnFailureResponse_WhenCreationFails()
    {
        var item = new WriteOnlyDtoConcrete();
        var dataHandlerResponse = new ResponseObj<WriteOnlyEntityConcrete> { IsSuccess = false, Message = "Error occurred" };

        _mockDataHandler
            .Setup(m => m.CreateItemAsync(It.IsAny<WriteOnlyEntityConcrete>(), default))
            .ReturnsAsync(dataHandlerResponse);

        var result = await _handler.CreateItemAsync(item, default);

        Assert.False(result.IsSuccess);
        Assert.Equal("Error occurred", result.Message);
        Assert.Equal(item, result.Data);

        _mockDataHandler.Verify(m => m.CreateItemAsync(It.IsAny<WriteOnlyEntityConcrete>(), default), Times.Once);
    }

    /// <summary>
    /// When creating data, if requested data is null, it throws exception.
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ShouldThrowArgumentException_WhenItemIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.CreateItemAsync(null!, default));
    }

    /// <summary>
    /// When data is created in bulk, it returns success with newly created data.
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ShouldReturnSuccessResponse_WhenItemsAreCreatedSuccessfully()
    {
        var items = new List<WriteOnlyDtoConcrete> { new(), new() };
        var entities = new List<WriteOnlyEntityConcrete> { new(), new() };
        var dataHandlerResponse = new ResponseObj<ICollection<WriteOnlyEntityConcrete>> { IsSuccess = true, Data = entities };

        _mockDataHandler
            .Setup(m => m.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyEntityConcrete>>(), default))
            .ReturnsAsync(dataHandlerResponse);

        var result = await _handler.CreateItemsAsync(items, default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);

        _mockDataHandler.Verify(m => m.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyEntityConcrete>>(), default), Times.Once);
    }

    /// <summary>
    /// When data (bulk) is not created, it returns failure with message.
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ShouldReturnFailureResponse_WhenCreationFails()
    {
        var items = new List<WriteOnlyDtoConcrete> { new(), new() };
        var dataHandlerResponse = new ResponseObj<ICollection<WriteOnlyEntityConcrete>> { IsSuccess = false, Message = "Error occurred" };

        _mockDataHandler
            .Setup(m => m.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyEntityConcrete>>(), default))
            .ReturnsAsync(dataHandlerResponse);

        var result = await _handler.CreateItemsAsync(items, default);

        Assert.False(result.IsSuccess);
        Assert.Equal("Error occurred", result.Message);
        Assert.Equal(items, result.Data);

        _mockDataHandler.Verify(m => m.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyEntityConcrete>>(), default), Times.Once);
    }

    /// <summary>
    /// When creating data (bulk), if requested data is null, it throws exception.
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ShouldThrowArgumentException_WhenItemsAreNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.CreateItemsAsync(null!, default));
    }
}