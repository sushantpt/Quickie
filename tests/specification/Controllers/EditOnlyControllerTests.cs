using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickie.Apis.Editonly;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.DataObj;
using Quickie.Handlers.Editonly.Definition;
using specification.Helpers.Dtos;

namespace specification.Controllers;

/// <summary>
/// Tests for Edit-Only controller.
/// </summary>
public class EditOnlyControllerTests
{
    private readonly Mock<IEditOnlyRequestHandler<EditOnlyDtoConcrete, int>> _mockHandler;
    private readonly EditOnlyController<EditOnlyDtoConcrete, IEditOnlyRequestHandler<EditOnlyDtoConcrete, int>, int> _controller;

    public EditOnlyControllerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);

        _mockHandler = new Mock<IEditOnlyRequestHandler<EditOnlyDtoConcrete, int>>();
        _controller = new EditOnlyController<EditOnlyDtoConcrete, IEditOnlyRequestHandler<EditOnlyDtoConcrete, int>, int>(_mockHandler.Object);
    }

    /// <summary>
    /// successful edit of a single item
    /// </summary>
    [Fact]
    public async Task EditAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var itemToEdit = new EditOnlyDtoConcrete();
        var expectedResponse = new ResponseObj<EditOnlyDtoConcrete>
        {
            IsSuccess = true,
            Data = itemToEdit
        };

        _mockHandler
            .Setup(h => h.EditAsync(It.IsAny<int>(), It.IsAny<EditOnlyDtoConcrete>(), It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.EditAsync(1, itemToEdit);

        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<EditOnlyDtoConcrete>>(okResult.Value);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(itemToEdit, responseObj.Data);

        _mockHandler.Verify(h => h.EditAsync(It.IsAny<int>(), It.IsAny<EditOnlyDtoConcrete>(), It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// edit fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task EditAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var itemToEdit = new EditOnlyDtoConcrete();
        var expectedResponse = new ResponseObj<EditOnlyDtoConcrete>
        {
            IsSuccess = false,
            Message = "Edit failed",
            Data = itemToEdit
        };

        _mockHandler
            .Setup(h => h.EditAsync(It.IsAny<int>(), It.IsAny<EditOnlyDtoConcrete>(), It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.EditAsync(1, itemToEdit);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<EditOnlyDtoConcrete>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Edit failed", responseObj.Message);

        _mockHandler.Verify(h => h.EditAsync(It.IsAny<int>(), It.IsAny<EditOnlyDtoConcrete>(), It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// invalid model state returns bad request
    /// </summary>
    [Fact]
    public async Task EditAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var controller = new EditOnlyController<EditOnlyDtoConcrete, IEditOnlyRequestHandler<EditOnlyDtoConcrete, int>, int>(_mockHandler.Object);
        controller.ModelState.AddModelError("SOmeProperty", "Validation error");

        var itemToEdit = new EditOnlyDtoConcrete();

        // act
        var result = await controller.EditAsync(1, itemToEdit);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<EditOnlyDtoConcrete>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Invalid request", responseObj.Message);
    }

    /// <summary>
    /// Successful edit of a collection of items
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_ValidRequest_ReturnsOkResponse()
    {
        // arrange
        var itemsToEdit = new List<EditOnlyDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<EditOnlyDtoConcrete>>
        {
            IsSuccess = true,
            Data = itemsToEdit
        };

        _mockHandler
            .Setup(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyDtoConcrete>>(), It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.EditCollectionAsync(itemsToEdit);

        // assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<EditOnlyDtoConcrete>>>(okResult.Value);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseObj.IsSuccess);
        Assert.Equal(itemsToEdit, responseObj.Data);

        _mockHandler
            .Verify(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyDtoConcrete>>(), It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// edit collection fails due to handler returning unsuccessful response
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_HandlerReturnsUnsuccessful_ReturnsBadRequest()
    {
        // arrange
        var itemsToEdit = new List<EditOnlyDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<EditOnlyDtoConcrete>>
        {
            IsSuccess = false,
            Message = "Edit collection failed",
            Data = itemsToEdit
        };

        _mockHandler
            .Setup(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyDtoConcrete>>(), It.IsAny<CancellationToken?>())).ReturnsAsync(expectedResponse);

        // act
        var result = await _controller.EditCollectionAsync(itemsToEdit);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<EditOnlyDtoConcrete>>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Equal("Edit collection failed", responseObj.Message);

        _mockHandler.Verify(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyDtoConcrete>>(), It.IsAny<CancellationToken?>()), Times.Once);
    }

    /// <summary>
    /// Invalid model state for collection edit returns bad request
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_InvalidModelState_ReturnsBadRequest()
    {
        // arrange
        var controller = new EditOnlyController<EditOnlyDtoConcrete, IEditOnlyRequestHandler<EditOnlyDtoConcrete, int>, int>(_mockHandler.Object);
        controller.ModelState.AddModelError("PropertyName", "Validation error");

        var itemsToEdit = new List<EditOnlyDtoConcrete> { new(), new() };

        // act
        var result = await controller.EditCollectionAsync(itemsToEdit);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var responseObj = Assert.IsType<ResponseObj<ICollection<EditOnlyDtoConcrete>>>(badRequestResult.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(responseObj.IsSuccess);
        Assert.Contains("Invalid request", responseObj.Message);
    }
}