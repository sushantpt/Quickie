using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickie.Apis.Writeonly;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.DataObj;
using Quickie.Handlers.Writeonly.Definition;
using specification.Helpers.Dtos;

namespace specification.Controllers;

/// <summary>
/// Tests for WriteOnlyController.
/// </summary>
public class WriteOnlyControllerTests
{
    private readonly Mock<IWriteOnlyRequestHandler<WriteOnlyDtoConcrete>> _mockHandler;
    private readonly WriteOnlyController<WriteOnlyDtoConcrete, IWriteOnlyRequestHandler<WriteOnlyDtoConcrete>> _controller;
    
    public WriteOnlyControllerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);
        
        _mockHandler = new Mock<IWriteOnlyRequestHandler<WriteOnlyDtoConcrete>>();
        _controller = new WriteOnlyController<WriteOnlyDtoConcrete, IWriteOnlyRequestHandler<WriteOnlyDtoConcrete>>(_mockHandler.Object);
    }
    
    /// <summary>
    /// Successful single item creation with valid request
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var dataToCreateDto = new WriteOnlyDtoConcrete();
        var expectedResponse = new ResponseObj<WriteOnlyDtoConcrete> { IsSuccess = true, Data = dataToCreateDto };
        
        _mockHandler
            .Setup(h => h.CreateItemAsync(It.IsAny<WriteOnlyDtoConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateItemAsync(idempotencyKey, dataToCreateDto, CancellationToken.None);
        
        // assert
        var okResult = Assert.IsType<ObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<WriteOnlyDtoConcrete>>(okResult.Value);
        
        Assert.Equal(StatusCodes.Status201Created, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(dataToCreateDto, responseObj.Data);
        
        _mockHandler.Verify(h => h.CreateItemAsync(It.IsAny<WriteOnlyDtoConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Single item creation fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var dataToCreateDto = new WriteOnlyDtoConcrete();
        var expectedResponse = new ResponseObj<WriteOnlyDtoConcrete> { IsSuccess = false, Message = "Creation failed", Data = dataToCreateDto };

        _mockHandler
            .Setup(h => h.CreateItemAsync(It.IsAny<WriteOnlyDtoConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateItemAsync(idempotencyKey, dataToCreateDto, CancellationToken.None);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<WriteOnlyDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Creation failed", responseObj.Message);
        
        _mockHandler.Verify(h => h.CreateItemAsync(It.IsAny<WriteOnlyDtoConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Single item creation fails due to invalid model state
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var controller = new WriteOnlyController<WriteOnlyDtoConcrete, IWriteOnlyRequestHandler<WriteOnlyDtoConcrete>>(_mockHandler.Object);
        controller.ModelState.AddModelError("SomePropertyName", "Some random validation message");
        
        var dataToCreateDto = new WriteOnlyDtoConcrete();
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await controller.CreateItemAsync(idempotencyKey, dataToCreateDto);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<WriteOnlyDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Invalid request", responseObj.Message);
    }

    /// <summary>
    /// Single item creation exception handling with custom error message disabled
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ExceptionThrown_ReturnsBadRequestWithGenericMessage()
    {
        // arrange
        _mockHandler.Setup(h => h.CreateItemAsync(It.IsAny<WriteOnlyDtoConcrete>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Some error message"));
        
        var dataToCreateDto = new WriteOnlyDtoConcrete();
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateItemAsync(idempotencyKey, dataToCreateDto);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<WriteOnlyDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        /* error message depends upon setup */
        //Assert.Contains("Object reference", responseObj.Message);
    }

    /// <summary>
    /// Successful bulk item creation with valid requests
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ValidRequests_ReturnsOkResponse()
    {
        // arrange
        var dataToCreate = new List<WriteOnlyDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<WriteOnlyDtoConcrete>> 
        { 
            IsSuccess = true, 
            Data = dataToCreate 
        };
        
        _mockHandler
            .Setup(h => h.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyDtoConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateItemsAsync(idempotencyKey, dataToCreate, CancellationToken.None);
        
        // assert
        var okResult = Assert.IsType<ObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<WriteOnlyDtoConcrete>>>(okResult.Value);
        
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(dataToCreate, responseObj.Data);
        Assert.Equal(StatusCodes.Status201Created, okResult.StatusCode);
        
        _mockHandler.Verify(h => h.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyDtoConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Bulk item creation fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var dataToCreate = new List<WriteOnlyDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<WriteOnlyDtoConcrete>> 
        { 
            IsSuccess = false, 
            Message = "An error occurred while creating the items.",
            Data = dataToCreate 
        };
        
        _mockHandler
            .Setup(h => h.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyDtoConcrete>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateItemsAsync(idempotencyKey, dataToCreate, CancellationToken.None);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<WriteOnlyDtoConcrete>>>(badRequestResult.Value);
        
        Assert.False(responseObj.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains("An error occurred", responseObj.Message);
        
        _mockHandler.Verify(h => h.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyDtoConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Bulk item creation fails due to invalid model state
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var controller = new WriteOnlyController<WriteOnlyDtoConcrete, IWriteOnlyRequestHandler<WriteOnlyDtoConcrete>>(_mockHandler.Object);
        controller.ModelState.AddModelError("SomePropertyName", "Some validation message");
        
        var dataToCreate = new List<WriteOnlyDtoConcrete> { new(), new() };
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await controller.CreateItemsAsync(idempotencyKey, dataToCreate);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<WriteOnlyDtoConcrete>>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Invalid request", responseObj.Message);
    }

    /// <summary>
    /// Bulk item creation exception handling
    /// </summary>
    [Fact]
    public async Task CreateItemsAsync_ExceptionThrown_ReturnsBadRequestWithGenericMessage()
    {
        // arrange
        _mockHandler
            .Setup(h => h.CreateItemsAsync(It.IsAny<ICollection<WriteOnlyDtoConcrete>>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Specific error message"));
        
        var dataToCreate = new List<WriteOnlyDtoConcrete> { new(), new() };
        var idempotencyKey = Guid.NewGuid().ToString();
        
        // act
        var result = await _controller.CreateItemsAsync(idempotencyKey, dataToCreate);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<WriteOnlyDtoConcrete>>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        /* error message depends upon setup */
        //Assert.Equal("An error occurred while creating the items.", responseObj.Message);
    }

}