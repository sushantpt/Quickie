using Moq;
using Quickie.Configuration;
using Quickie.Configuration.Options;
using Quickie.DataHandlers.Crud.Definition;
using Quickie.DataObj;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;
using specification.Helpers.RequestHandlers;

namespace specification.RequestHandlers;

/// <summary>
/// Tests for crud request handler.
/// </summary>
public class CrudRequestHandlerTests
{
    private readonly CrudRequestHandlerConcrete _crudRequestHandler;
    private readonly Mock<ICrudDataHandler<CrudEntityConcrete,int>> _mockCrudDataHandler;

    public CrudRequestHandlerTests()
    {
        var quickieOptions = new QuickieOptions
        {
            ShowCustomErrorMessage = false,
        };
        GlobalQuickieConfigData.Initialize(quickieOptions);
        
        _mockCrudDataHandler = new Mock<ICrudDataHandler<CrudEntityConcrete,int>>();
        _crudRequestHandler = new CrudRequestHandlerConcrete(_mockCrudDataHandler.Object);
    }
    
    /// <summary>
    /// Create successful tests returning proper data.
    /// </summary>
    [Fact]
    public async Task CreateAsync_SuccessfulCreation_ReturnsResponseWithData()
    {
        var dtoToCreate = new CrudDtoConcrete();
        var expectedEntity = new CrudEntityConcrete();
        var expectedResponse = new ResponseObj<CrudEntityConcrete> { IsSuccess = true, Data = expectedEntity };

        _mockCrudDataHandler.Setup(h => h.CreateAsync(It.IsAny<CrudEntityConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.CreateAsync(dtoToCreate, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(dtoToCreate, result.Data);
        
        _mockCrudDataHandler.Verify(h => h.CreateAsync(It.IsAny<CrudEntityConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Create failed tests returning proper data.
    /// </summary>
    [Fact]
    public async Task CreateAsync_UnsuccessfulCreation_ReturnsUnsuccessfulResponse()
    {
        var dtoToCreate = new CrudDtoConcrete();
        var expectedResponse = new ResponseObj<CrudEntityConcrete> { IsSuccess = false, Message = "Creation failed" };

        _mockCrudDataHandler.Setup(h => h.CreateAsync(It.IsAny<CrudEntityConcrete>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.CreateAsync(dtoToCreate, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Creation failed", result.Message);
    }
    
    /// <summary>
    /// Create failed (throws exception) for null data.
    /// </summary>
    [Fact]
    public async Task CreateAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _crudRequestHandler.CreateAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Update successful tests returning proper data.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_SuccessfulUpdate_ReturnsResponseWithData()
    {
        const int id = 1;
        var dtoToUpdate = new CrudDtoConcrete();
        var expectedEntity = new CrudEntityConcrete();
        var expectedResponse = new ResponseObj<CrudEntityConcrete> { IsSuccess = true, Data = expectedEntity };

        _mockCrudDataHandler.Setup(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudEntityConcrete>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.UpdateAsync(id, dtoToUpdate);

        Assert.True(result.IsSuccess);
        Assert.Equal(dtoToUpdate, result.Data);
        
        _mockCrudDataHandler.Verify(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudEntityConcrete>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    /// <summary>
    /// Update unsuccessful with proper data (message).
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UnsuccessfulUpdate_ReturnsUnsuccessfulResponse()
    {
        const int id = 1;
        var dtoToUpdate = new CrudDtoConcrete();
        var expectedResponse = new ResponseObj<CrudEntityConcrete> { IsSuccess = false, Message = "Update failed" };

        _mockCrudDataHandler
            .Setup(h => h.UpdateAsync(It.Is<int>(x => x == id), It.IsAny<CrudEntityConcrete>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.UpdateAsync(id, dtoToUpdate);

        Assert.False(result.IsSuccess);
        Assert.Contains("Update failed", result.Message);
    }
    
    /// <summary>
    /// Update unsuccessful (throws exception) for null data.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _crudRequestHandler.UpdateAsync(1, null!));
    }
    
    /// <summary>
    /// Get data (success).
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_SuccessfulRetrieval_ReturnsResponseWithData()
    {
        const int id = 1;
        var expectedResponse = new ResponseObj<CrudEntityConcrete> { IsSuccess = true, Data = new CrudEntityConcrete() };

        _mockCrudDataHandler.Setup(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var mappedResponse = await _crudRequestHandler.GetByIdAsync(id);

        Assert.True(mappedResponse.IsSuccess);
        Assert.NotNull(mappedResponse.Data);
        
        _mockCrudDataHandler.Verify(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    /// <summary>
    /// Get data but data not found.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NotFoundEntity_ReturnsUnsuccessfulResponse()
    {
        const int id = 1;
        var expectedResponse = new ResponseObj<CrudEntityConcrete> { IsSuccess = false, Message = "Not found", /*Data = null!*/ }; // FindAsync returns null when not found

        _mockCrudDataHandler
            .Setup(h => h.GetByIdAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.GetByIdAsync(id);

        Assert.False(result.IsSuccess);
        Assert.Contains("Not found", result.Message);
        Assert.Equal(null!, result.Data);
    }
    
    /// <summary>
    /// Get data throw exception when invalid (default) id is passed. 
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DefaultId_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _crudRequestHandler.GetByIdAsync(default));
    }
    
    /// <summary>
    /// Get data throws exception when invalid (-ve int) id is passed
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NegativeId_ReturnsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _crudRequestHandler.GetByIdAsync(-1));
    }

    /// <summary>
    /// Delete is successful.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_SuccessfulDeletion_ReturnsSuccessfulResponse()
    {
        const int id = 78;
        var expectedResponse = new ResponseObj { IsSuccess = true };

        _mockCrudDataHandler
            .Setup(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.DeleteAsync(id);

        Assert.True(result.IsSuccess);
        
        _mockCrudDataHandler.Verify(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Delete fails.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_UnsuccessfulDeletion_ReturnsUnsuccessfulResponse()
    {
        const int id = 1;
        var expectedResponse = new ResponseObj { IsSuccess = false, Message = "Delete failed" };

        _mockCrudDataHandler
            .Setup(h => h.DeleteAsync(It.Is<int>(x => x == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _crudRequestHandler.DeleteAsync(id);

        Assert.False(result.IsSuccess);
        Assert.Contains("Delete failed", result.Message);
    }
    
    /// <summary>
    /// Delete fails due to invalid (default) id is passed.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DefaultId_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _crudRequestHandler.DeleteAsync(0));
    }
}