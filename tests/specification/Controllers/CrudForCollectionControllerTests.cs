using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickie.Apis.Crud;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.DataObj;
using Quickie.Handlers.Crud.Definition;
using specification.Helpers.Dtos;

namespace specification.Controllers;

/// <summary>
/// Tests for Collection CRUD controller.
/// </summary>
public class CrudForCollectionControllerTests
{
    private readonly Mock<ICrudForCollectionRequestHandler<CrudDtoConcrete, int>> _mockHandler;
    private readonly CrudForCollectionController<CrudDtoConcrete, ICrudForCollectionRequestHandler<CrudDtoConcrete, int>, int>
        _controller;

    public CrudForCollectionControllerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);

        _mockHandler = new Mock<ICrudForCollectionRequestHandler<CrudDtoConcrete, int>>();
        _controller = new CrudForCollectionController<CrudDtoConcrete, ICrudForCollectionRequestHandler<CrudDtoConcrete, int>, int>(_mockHandler
                .Object);
    }

    /// <summary>
    /// successful creation of a collection of data
    /// </summary>
    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedResponse()
    {
        // arrange
        var dataToCreate = new List<CrudDtoConcrete> { new(), new () };
        var expectedResponse = new ResponseObj<ICollection<CrudDtoConcrete>>
        {
            IsSuccess = true,
            Data = dataToCreate
        };

        _mockHandler
            .Setup(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        var idempotencyKey = Guid.NewGuid().ToString();

        // act
        var result = await _controller.CreateAsync(idempotencyKey, dataToCreate);

        // assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<CrudDtoConcrete>>>(objectResult.Value);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(dataToCreate, responseObj.Data);

        _mockHandler.Verify(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// creation fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task CreateAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var dataToCreate = new List<CrudDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<CrudDtoConcrete>>
        {
            IsSuccess = false,
            Message = "Creation failed",
            Data = dataToCreate
        };

        _mockHandler
            .Setup(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        var idempotencyKey = Guid.NewGuid().ToString();

        // act
        var result = await _controller.CreateAsync(idempotencyKey, dataToCreate);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<CrudDtoConcrete>>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Creation failed", responseObj.Message);

        _mockHandler.Verify(h => h.CreateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// invalid model state returns bad request
    /// </summary>
    [Fact]
    public async Task CreateAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var controller = new CrudForCollectionController<CrudDtoConcrete, ICrudForCollectionRequestHandler<CrudDtoConcrete, int>, int>(_mockHandler.Object);
        controller.ModelState.AddModelError("PropertyName", "Validation error");

        var dataToCreate = new List<CrudDtoConcrete> { new(), new() };
        var idempotencyKey = Guid.NewGuid().ToString();

        // act
        var result = await controller.CreateAsync(idempotencyKey, dataToCreate);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<CrudDtoConcrete>>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Invalid requests", responseObj.Message);
    }

    /// <summary>
    /// successful update of a collection of data
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var dataToUpdate = new List<CrudDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<CrudDtoConcrete>>
        {
            IsSuccess = true,
            Data = dataToUpdate
        };

        _mockHandler
            .Setup(h => h.UpdateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.UpdateAsync(dataToUpdate);

        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<CrudDtoConcrete>>>(okResult.Value);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(dataToUpdate, responseObj.Data);

        _mockHandler.Verify(h => h.UpdateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// update fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task UpdateAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var dataToUpdate = new List<CrudDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<CrudDtoConcrete>>
        {
            IsSuccess = false,
            Message = "Update failed",
            Data = dataToUpdate
        };

        _mockHandler
            .Setup(h => h.UpdateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.UpdateAsync(dataToUpdate);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<CrudDtoConcrete>>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Update failed", responseObj.Message);

        _mockHandler.Verify(h => h.UpdateRangeAsync(It.IsAny<ICollection<CrudDtoConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// successful retrieval of paginated data
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var request = new RequestForDataObj();
        var expectedResponse = new ResponseObj<PaginatedDataObj<CrudDtoConcrete>>
        {
            IsSuccess = true,
            Data = new PaginatedDataObj<CrudDtoConcrete>
            {
                Items = new List<CrudDtoConcrete> { new(), new() }
            }
        };

        _mockHandler
            .Setup(h => h.GetAllAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.GetAllAsync(request);

        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<PaginatedDataObj<CrudDtoConcrete>>>(okResult.Value);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.NotNull(responseObj.Data);

        _mockHandler.Verify(h => h.GetAllAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// get all fails when handler returns unsuccessful response
    /// </summary>
    [Fact]
    public async Task GetAllAsync_HandlerReturnsUnsuccessful_ReturnsNotFound()
    {
        // arrange
        var request = new RequestForDataObj();
        var expectedResponse = new ResponseObj<PaginatedDataObj<CrudDtoConcrete>>
        {
            IsSuccess = false,
            Message = "No data found"
        };

        _mockHandler
            .Setup(h => h.GetAllAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.GetAllAsync(request);

        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<PaginatedDataObj<CrudDtoConcrete>>>(notFoundResult.Value);

        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("No data found", responseObj.Message);

        _mockHandler.Verify(h => h.GetAllAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// successful count retrieval
    /// </summary>
    [Fact]
    public async Task CountAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var expectedResponse = new ResponseObj<int>
        {
            IsSuccess = true,
            Data = 10
        };

        _mockHandler
            .Setup(h => h.CountAsync(It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.CountAsync();

        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<int>>(okResult.Value);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(10, responseObj.Data);

        _mockHandler.Verify(h => h.CountAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// count retrieval fails when handler returns unsuccessful response
    /// </summary>
    [Fact]
    public async Task CountAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var expectedResponse = new ResponseObj<int>
        {
            IsSuccess = false,
            Message = "Count failed",
            Data = 0
        };

        _mockHandler
            .Setup(h => h.CountAsync(It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.CountAsync();

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<int>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Count failed", responseObj.Message);

        _mockHandler.Verify(h => h.CountAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// successful deletion of a collection of data
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ValidRequest_ReturnsNoContent()
    {
        // arrange
        var idsToDelete = new List<int> { 1, 2, 3 };
        var expectedResponse = new ResponseObj { IsSuccess = true };

        _mockHandler
            .Setup(h => h.DeleteRangeAsync(It.IsAny<ICollection<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.DeleteAsync(idsToDelete);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);

        _mockHandler.Verify(h => h.DeleteRangeAsync(It.IsAny<ICollection<int>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// delete fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task DeleteAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var idsToDelete = new List<int> { 1, 2, 3 };
        var expectedResponse = new ResponseObj
        {
            IsSuccess = false,
            Message = "Delete failed"
        };

        _mockHandler
            .Setup(h => h.DeleteRangeAsync(It.IsAny<ICollection<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.DeleteAsync(idsToDelete);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }
}