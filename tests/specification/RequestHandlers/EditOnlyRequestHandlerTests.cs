using Moq;
using Quickie.DataHandlers.Editonly.Definition;
using Quickie.DataObj;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;
using specification.Helpers.RequestHandlers;

namespace specification.RequestHandlers;

/// <summary>
/// Tests for EditOnlyRequestHandler.
/// </summary>
public class EditOnlyRequestHandlerTests
{
    private readonly EditOnlyRequestHandlerConcrete _editOnlyRequestHandler;
    private readonly Mock<IEditOnlyDataHandler<EditOnlyEntityConcrete, int>> _mockDataHandler;

    public EditOnlyRequestHandlerTests()
    {
        _mockDataHandler = new Mock<IEditOnlyDataHandler<EditOnlyEntityConcrete, int>>();
        _editOnlyRequestHandler = new EditOnlyRequestHandlerConcrete(_mockDataHandler.Object);
    }

    /// <summary>
    /// EditAsync successful test returning proper data.
    /// </summary>
    [Fact]
    public async Task EditAsync_SuccessfulEdit_ReturnsResponseWithData()
    {
        const int id = 1;
        var dtoToEdit = new EditOnlyDtoConcrete();
        var updatedEntity = new EditOnlyEntityConcrete();
        var expectedResponse = new ResponseObj<EditOnlyEntityConcrete> { IsSuccess = true, Data = updatedEntity };

        _mockDataHandler
            .Setup(h => h.EditAsync(id, It.IsAny<EditOnlyEntityConcrete>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _editOnlyRequestHandler.EditAsync(id, dtoToEdit, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        _mockDataHandler.Verify(h => h.EditAsync(id, It.IsAny<EditOnlyEntityConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// EditAsync unsuccessful test returning proper message.
    /// </summary>
    [Fact]
    public async Task EditAsync_UnsuccessfulEdit_ReturnsUnsuccessfulResponse()
    {
        const int id = 1;
        var dtoToEdit = new EditOnlyDtoConcrete();
        var mappedEntity = new EditOnlyEntityConcrete(); // dto mapped to entity
        var expectedResponse = new ResponseObj<EditOnlyEntityConcrete> { IsSuccess = false, Message = "Edit failed", Data = mappedEntity };

        _mockDataHandler
            .Setup(h => h.EditAsync(id, It.IsAny<EditOnlyEntityConcrete>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _editOnlyRequestHandler.EditAsync(id, dtoToEdit, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Edit failed", result.Message);

        _mockDataHandler.Verify(h => h.EditAsync(id, It.IsAny<EditOnlyEntityConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// EditAsync throws ArgumentNullException for null request.
    /// </summary>
    [Fact]
    public async Task EditAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _editOnlyRequestHandler.EditAsync(1, null!, CancellationToken.None));
    }

    /// <summary>
    /// EditCollectionAsync successful test returning proper data.
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_SuccessfulEdit_ReturnsResponseWithData()
    {
        var dtoToEdit = new List<EditOnlyDtoConcrete> { new(), new() };
        var updatedEntities = new List<EditOnlyEntityConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<EditOnlyEntityConcrete>> { IsSuccess = true, Data = updatedEntities };

        _mockDataHandler
            .Setup(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyEntityConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _editOnlyRequestHandler.EditCollectionAsync(dtoToEdit, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        _mockDataHandler.Verify(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyEntityConcrete>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// EditCollectionAsync unsuccessful test returning proper message.
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_UnsuccessfulEdit_ReturnsUnsuccessfulResponse()
    {
        var dtoToEdit = new List<EditOnlyDtoConcrete> { new(), new() };
        var expectedResponse = new ResponseObj<ICollection<EditOnlyEntityConcrete>> { IsSuccess = false, Message = "Edit collection failed" };

        _mockDataHandler
            .Setup(h => h.EditCollectionAsync(It.IsAny<ICollection<EditOnlyEntityConcrete>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _editOnlyRequestHandler.EditCollectionAsync(dtoToEdit, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Edit collection failed", result.Message);
    }

    /// <summary>
    /// EditCollectionAsync throws ArgumentNullException for null request.
    /// </summary>
    [Fact]
    public async Task EditCollectionAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _editOnlyRequestHandler.EditCollectionAsync(null!, CancellationToken.None));
    }
}