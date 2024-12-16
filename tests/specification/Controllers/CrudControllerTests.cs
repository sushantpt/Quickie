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
/// Tests for CRUD controller.
/// </summary>
public class CrudControllerTests
{
    private readonly Mock<ICrudRequestHandler<CrudDtoConcrete, int>> _mockHandler;
    private readonly CrudController<CrudDtoConcrete, ICrudRequestHandler<CrudDtoConcrete, int>, int> _controller;
    
    public CrudControllerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);
        
        _mockHandler = new Mock<ICrudRequestHandler<CrudDtoConcrete, int>>();
        _controller = new CrudController<CrudDtoConcrete, ICrudRequestHandler<CrudDtoConcrete, int>, int>(_mockHandler.Object);
    }
    
    /// <summary>
    /// successful creation with valid request
    /// </summary>
    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedResponse()
    {
        // arrange
        var dataToCreateDto = new CrudDtoConcrete();
        var mockResponse = new ResponseObj<CrudDtoConcrete> { IsSuccess = true, Data = dataToCreateDto };
        // mocking request layer
        _mockHandler
            .Setup(h => h.CreateAsync(It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateAsync(idempotencyKey, dataToCreateDto);
        
        // assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(objectResult.Value);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(dataToCreateDto, responseObj.Data);
        
        _mockHandler
            .Verify(h => h.CreateAsync(It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// creation fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task CreateAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var dataToCreateDto = new CrudDtoConcrete();
        var mockResponse = new ResponseObj<CrudDtoConcrete> { IsSuccess = false, Message = "Creation failed", Data = dataToCreateDto };

        _mockHandler
            .Setup(h => h.CreateAsync(It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateAsync(idempotencyKey, dataToCreateDto);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(badRequestResult.Value);
        
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Creation failed", responseObj.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        
        _mockHandler
            .Verify(h => h.CreateAsync(It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// invalid model state returns bad request
    /// </summary>
    [Fact]
    public async Task CreateAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var controller = new CrudController<CrudDtoConcrete, ICrudRequestHandler<CrudDtoConcrete, int>, int>(_mockHandler.Object);
        controller.ModelState.AddModelError("PropertyName", "Validation error");
        
        var dataToCreateDto = new CrudDtoConcrete();
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await controller.CreateAsync(idempotencyKey, dataToCreateDto);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Invalid requests", responseObj.Message);
    }

    /// <summary>
    /// exception handling test with custom error message disabled
    /// </summary>
    [Fact]
    public async Task CreateAsync_ExceptionThrown_ReturnsBadRequestWithGenericMessage()
    {
        // arrange
        _mockHandler
            .Setup(h => h.CreateAsync(It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("some error"));
        
        var dataToCreateDto = new CrudDtoConcrete();
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateAsync(idempotencyKey, dataToCreateDto);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Failed to create request.", responseObj.Message);
    }

    /// <summary>
    /// successful update with valid request
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        const int id = 1;
        var dataToUpdateDto = new CrudDtoConcrete();
        var expectedResponse = new ResponseObj<CrudDtoConcrete> { IsSuccess = true, Message = "Updated", Data = dataToUpdateDto };
        
        _mockHandler
            .Setup(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.UpdateAsync(id, dataToUpdateDto);
        
        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(okResult.Value);
        
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(dataToUpdateDto, responseObj.Data);
        Assert.Contains("Updated", responseObj.Message);
        
        _mockHandler
            .Verify(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    /// <summary>
    /// update fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task UpdateAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        const int id = 1;
        var dataToUpdateDto = new CrudDtoConcrete();
        var expectedResponse = new ResponseObj<CrudDtoConcrete> { IsSuccess = false, Message = "Update failed", Data = dataToUpdateDto };

        _mockHandler
            .Setup(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.UpdateAsync(id, dataToUpdateDto);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Update failed", responseObj.Message);
        
        _mockHandler
            .Verify(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudDtoConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    /// <summary>
    /// successful get by id
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsOkResponse()
    {
        // arrange
        const int id = 1;
        var expectedDto = new CrudDtoConcrete();
        var expectedResponse = new ResponseObj<CrudDtoConcrete> { IsSuccess = true, Data = expectedDto };
        
        _mockHandler
            .Setup(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.GetByIdAsync(id);
        
        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(okResult.Value);
        
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(expectedDto, responseObj.Data);
        
        _mockHandler
            .Verify(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// get by id returns not found when handler returns unsuccessful
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNotFoundResponse()
    {
        // arrange
        const int id = 1;
        var expectedResponse = new ResponseObj<CrudDtoConcrete> { IsSuccess = false, Message = "Not found" };

        _mockHandler
            .Setup(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.GetByIdAsync(id);
        
        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<CrudDtoConcrete>>(notFoundResult.Value);
        
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Not found", responseObj.Message);
        
        _mockHandler
            .Verify(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// successful delete
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ValidId_ReturnsNoContent()
    {
        // arrange
        const int id = 1;
        var expectedResponse = new ResponseObj { IsSuccess = true };
        
        _mockHandler
            .Setup(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.DeleteAsync(id);
        
        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        
        _mockHandler.Verify(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// delete fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task DeleteAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        const int id = 1;
        var expectedResponse = new ResponseObj { IsSuccess = false, Message = "Delete failed" };

        _mockHandler
            .Setup(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.DeleteAsync(id);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Delete failed", responseObj.Message);
        
        _mockHandler.Verify(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }
    
}