using Moq;
using Quickie.DataHandlers.Crud.Definition;
using Quickie.DataObj;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;
using specification.Helpers.RequestHandlers;

namespace specification.RequestHandlers;

/// <summary>
/// Tests for the CrudForCollectionRequestHandler.
/// </summary>
public class CrudForCollectionRequestHandlerTests
{
    private readonly Mock<ICrudForCollectionDataHandler<CrudEntityConcrete, int>> _mockDataHandler;
    private readonly CrudForCollectionRequestHandlerConcrete _handler;

    public CrudForCollectionRequestHandlerTests()
    {
        _mockDataHandler = new Mock<ICrudForCollectionDataHandler<CrudEntityConcrete, int>>();
        _handler = new CrudForCollectionRequestHandlerConcrete(_mockDataHandler.Object);
    }

    /// <summary>
    /// CreateRangeAsync successful test.
    /// </summary>
    [Fact]
    public async Task CreateRangeAsync_SuccessfulCreation_ReturnsResponseWithData()
    {
        var dtoCollection = new List<CrudDtoConcrete> { new() };
        var entityCollection = new List<CrudEntityConcrete> { new() };
        var expectedResponse = new ResponseObj<ICollection<CrudEntityConcrete>> { IsSuccess = true, Data = entityCollection, Message = "Success"};

        _mockDataHandler.Setup(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudEntityConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.CreateRangeAsync(dtoCollection, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.Contains("Success", result.Message);
        _mockDataHandler.Verify(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudEntityConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// CreateRangeAsync failed test.
    /// </summary>
    [Fact]
    public async Task CreateRangeAsync_UnsuccessfulCreation_ReturnsUnsuccessfulResponse()
    {
        var dtoCollection = new List<CrudDtoConcrete> { new() };
        var expectedResponse = new ResponseObj<ICollection<CrudEntityConcrete>> { IsSuccess = false, Message = "Creation failed" };

        _mockDataHandler.Setup(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudEntityConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.CreateRangeAsync(dtoCollection, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Creation failed", result.Message);
    }

    /// <summary>
    /// UpdateRangeAsync successful test.
    /// </summary>
    [Fact]
    public async Task UpdateRangeAsync_SuccessfulUpdate_ReturnsResponseWithData()
    {
        var dtoCollection = new List<CrudDtoConcrete> { new() };
        var entityCollection = new List<CrudEntityConcrete> { new() };
        var expectedResponse = new ResponseObj<ICollection<CrudEntityConcrete>> { IsSuccess = true, Data = entityCollection };

        _mockDataHandler.Setup(h => h.UpdateRangeAsync(It.IsAny<ICollection<CrudEntityConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.UpdateRangeAsync(dtoCollection, CancellationToken.None);

        /* to check data, it should be mapped to dto */
        
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
    }

    /// <summary>
    /// UpdateRangeAsync failed test.
    /// </summary>
    [Fact]
    public async Task UpdateRangeAsync_UnsuccessfulUpdate_ReturnsUnsuccessfulResponse()
    {
        var dtoCollection = new List<CrudDtoConcrete> { new() };
        var expectedResponse = new ResponseObj<ICollection<CrudEntityConcrete>> { IsSuccess = false, Message = "Update failed" };

        _mockDataHandler.Setup(h => h.UpdateRangeAsync(It.IsAny<ICollection<CrudEntityConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.UpdateRangeAsync(dtoCollection, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Update failed", result.Message);
    }

    /// <summary>
    /// DeleteRangeAsync successful test.
    /// </summary>
    [Fact]
    public async Task DeleteRangeAsync_SuccessfulDeletion_ReturnsSuccessfulResponse()
    {
        var ids = new List<int> { 1, 2, 3 };
        var expectedResponse = new ResponseObj { IsSuccess = true };

        _mockDataHandler.Setup(h => h.DeleteRangeAsync(It.IsAny<ICollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.DeleteRangeAsync(ids, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// GetAllAsync successful test.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_SuccessfulRetrieval_ReturnsPaginatedData()
    {
        var request = new RequestForDataObj();
        var paginatedData = new PaginatedDataObj<CrudEntityConcrete>
        {
            Items = new List<CrudEntityConcrete> { new(), new(), new(), new() },
            Total = 12 // total no of data, not the count of items.
        };
        var expectedResponse = new ResponseObj<PaginatedDataObj<CrudEntityConcrete>> { IsSuccess = true, Data = paginatedData };

        _mockDataHandler.Setup(h => h.GetAllAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.GetAllAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Data.Total);
    }

    /// <summary>
    /// CountAsync successful test.
    /// </summary>
    [Fact]
    public async Task CountAsync_ReturnsCount()
    {
        var expectedResponse = new ResponseObj<int> { IsSuccess = true, Data = 42 };

        _mockDataHandler.Setup(h => h.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.CountAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Data);
    }

    /// <summary>
    /// DeleteRangeAsync failed due to null ids.
    /// </summary>
    [Fact]
    public async Task DeleteRangeAsync_NullIds_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.DeleteRangeAsync(null!));
    }

    /// <summary>
    /// CreateRangeAsync fails due to null input.
    /// </summary>
    [Fact]
    public async Task CreateRangeAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.CreateRangeAsync(null!));
    }
}