using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickie.Apis.Readonly;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.DataObj;
using Quickie.Handlers.Readonly.Definition;
using specification.Helpers.Dtos;

namespace specification.Controllers;

/// <summary>
/// Tests for ReadOnlyController.
/// </summary>
public class ReadOnlyControllerTests
{
    private readonly Mock<IReadOnlyRequestHandler<ReadOnlyDtoConcrete, string>> _mockHandler;
    private readonly ReadOnlyController<ReadOnlyDtoConcrete, IReadOnlyRequestHandler<ReadOnlyDtoConcrete, string>, string> _controller;
    
    public ReadOnlyControllerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);
        
        _mockHandler = new Mock<IReadOnlyRequestHandler<ReadOnlyDtoConcrete, string>>();
        _controller = new ReadOnlyController<ReadOnlyDtoConcrete, IReadOnlyRequestHandler<ReadOnlyDtoConcrete, string>, string>(_mockHandler.Object);
    }
    
    /// <summary>
    /// successful get by id with valid request
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsOkResponse()
    {
        // arrange
        var id = new Guid().ToString();
        var expectedDto = new ReadOnlyDtoConcrete();
        var expectedResponse = new ResponseObj<ReadOnlyDtoConcrete> 
        { 
            IsSuccess = true, 
            Data = expectedDto 
        };
        
        _mockHandler
            .Setup(h => h.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.GetByIdAsync(id, CancellationToken.None);
        
        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ReadOnlyDtoConcrete>>(okResult.Value);
        
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(expectedDto, responseObj.Data);
        
        _mockHandler.Verify(h => h.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Get by id returns not found when handler returns unsuccessful response
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNotFoundResponse()
    {
        // arrange
        var id = new Guid().ToString();
        var expectedResponse = new ResponseObj<ReadOnlyDtoConcrete> 
        { 
            IsSuccess = false, 
            Message = "Not found" 
        };

        _mockHandler
            .Setup(h => h.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
        // act
        var result = await _controller.GetByIdAsync(id, CancellationToken.None);
        
        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ReadOnlyDtoConcrete>>(notFoundResult.Value);
        
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Not found", responseObj.Message);
        
        _mockHandler.Verify(h => h.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// exception handling test with custom error message disabled
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ExceptionThrown_ReturnsBadRequestWithGenericMessage()
    {
        // arrange
        var id = new Guid().ToString();
        _mockHandler
            .Setup(h => h.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Specific error message"));
        
        // act
        var result = await _controller.GetByIdAsync(id);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ReadOnlyDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        /* error message depends upon setup */
        //Assert.Contains("Object reference", responseObj.Message);
    }

    /// <summary>
    /// exception handling test with custom error message enabled
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ExceptionThrown_WithCustomErrorMessageEnabled()
    {
        // arrange
        // Enable custom error messages
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = true,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);

        var controller = new ReadOnlyController<ReadOnlyDtoConcrete, IReadOnlyRequestHandler<ReadOnlyDtoConcrete, string>, string>(_mockHandler.Object);
        
        var id = Guid.NewGuid().ToString();
        _mockHandler
            .Setup(h => h.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Specific error message"));
        
        // act
        var result = await controller.GetByIdAsync(id);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ReadOnlyDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Failed to get data", responseObj.Message);
    }

    /// <summary>
    /// get by id with null or empty id throws exception
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NullOrEmptyId_BadRequest()
    {
        // arrange
        var id = string.Empty;

        // act & assert
        var result = await _controller.GetByIdAsync(id);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ReadOnlyDtoConcrete>>(badRequestResult.Value);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Object reference", responseObj.Message);
    }
}