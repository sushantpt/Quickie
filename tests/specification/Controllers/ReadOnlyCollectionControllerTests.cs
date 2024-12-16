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
/// Tests for ReadOnlyCollectionController.
/// </summary>
public class ReadOnlyCollectionControllerTests
{
    private readonly Mock<IReadOnlyCollectionRequestHandler<ReadOnlyDtoConcrete, string>> _mockHandler;
    private readonly ReadOnlyCollectionController<ReadOnlyDtoConcrete, IReadOnlyCollectionRequestHandler<ReadOnlyDtoConcrete, string>, RequestForDataObj, string> _controller;
    
    public ReadOnlyCollectionControllerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);
        
        _mockHandler = new Mock<IReadOnlyCollectionRequestHandler<ReadOnlyDtoConcrete, string>>();
        _controller = new ReadOnlyCollectionController<ReadOnlyDtoConcrete, IReadOnlyCollectionRequestHandler<ReadOnlyDtoConcrete, string>, RequestForDataObj, string>(_mockHandler.Object);
    }

    /// <summary>
    /// Successful GetAsync with valid request
    /// </summary>
    [Fact]
    public async Task GetAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var request = new RequestForDataObj();
        // mocking: request per page as 2. so, 2 data and total count
        var expectedPaginatedData = new PaginatedDataObj<ReadOnlyDtoConcrete>
        {
            Items = new List<ReadOnlyDtoConcrete> { new(), new() },
            Total = 10
        };
        
        _mockHandler
            .Setup(h => h.GetAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPaginatedData);
        
        // act
        var result = await _controller.GetAsync(request, CancellationToken.None);
        
        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var paginatedData = Assert.IsType<PaginatedDataObj<ReadOnlyDtoConcrete>>(okResult.Value);
        
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(2, paginatedData.Items.Count());
        Assert.Equal(10, paginatedData.Total);
        
        _mockHandler
            .Verify(h => h.GetAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// GetAsync returns NotFound when no items are found
    /// </summary>
    [Fact]
    public async Task GetAsync_NoItemsFound_ReturnsNotFoundResponse()
    {
        // arrange
        var request = new RequestForDataObj();
        var expectedPaginatedData = new PaginatedDataObj<ReadOnlyDtoConcrete>
        {
            Items = new List<ReadOnlyDtoConcrete>(),
            Total = 0
        };
        
        _mockHandler
            .Setup(h => h.GetAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedPaginatedData);
        
        // act
        var result = await _controller.GetAsync(request, CancellationToken.None);
        
        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Contains("No items found matching", notFoundResult.Value?.ToString());
    }

    /// <summary>
    /// GetAsync returns BadRequest when model state is invalid
    /// </summary>
    [Fact]
    public async Task GetAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var request = new RequestForDataObj();
        _controller.ModelState.AddModelError("SomeProperty", "Validation error");
        
        // act
        var result = await _controller.GetAsync(request);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains("Invalid request", badRequestResult.Value?.ToString());
    }

    /// <summary>
    /// GetAsync handles exceptions with appropriate error response
    /// </summary>
    [Fact]
    public async Task GetAsync_ExceptionThrown_ReturnsBadRequestWithErrorMessage()
    {
        // arrange
        var request = new RequestForDataObj();
        _mockHandler
            .Setup(h => h.GetAsync(It.IsAny<RequestForDataObj>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("some exception"));
        
        // act
        var result = await _controller.GetAsync(request);
        
        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        /* error message depends upon setup */
        //Assert.Contains("Object reference", badRequestResult.Value?.ToString());
    }
}