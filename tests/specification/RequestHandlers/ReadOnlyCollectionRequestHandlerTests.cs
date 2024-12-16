using System.Linq.Expressions;
using Moq;
using Quickie.DataHandlers.Readonly.Definition;
using Quickie.DataObj;
using specification.Helpers.Entities;
using specification.Helpers.RequestHandlers;

namespace specification.RequestHandlers;

/// <summary>
/// Tests for read only collection request handler
/// </summary>
public class ReadOnlyCollectionRequestHandlerTests
{
    private readonly ReadOnlyCollectionRequestHandlerConcrete _readOnlyForCollectionRequestHandler;
    private readonly Mock<IReadOnlyCollectionDataHandler<ReadOnlyEntityConcrete, int>> _mockDataHandler;

    public ReadOnlyCollectionRequestHandlerTests()
    {
        _mockDataHandler = new Mock<IReadOnlyCollectionDataHandler<ReadOnlyEntityConcrete, int>>();
        _readOnlyForCollectionRequestHandler = new ReadOnlyCollectionRequestHandlerConcrete(_mockDataHandler.Object);
    }

    /// <summary>
    /// GetAsync returns proper paginated data when exists
    /// </summary>
    [Fact]
    public async Task GetAsync_ShouldReturnPaginatedData_WhenDataExists()
    {
        var request = new RequestForDataObj { PageNumber = 1, PageSize = 2 };
        var testEntities = new List<ReadOnlyEntityConcrete> { new(), new() };

        var paginatedData = new PaginatedDataObj<ReadOnlyEntityConcrete> { Items = testEntities, Total = 56 };

        _mockDataHandler
            .Setup(m => m.GetByFilterAsync(It.IsAny<Expression<Func<ReadOnlyEntityConcrete, bool>>>(),
                request, false, default))
            .ReturnsAsync(paginatedData);
        
        var result = await _readOnlyForCollectionRequestHandler.GetAsync(request: request);

        Assert.NotNull(result);
        Assert.Equal(56, result.Total);

        _mockDataHandler.Verify(m => m.GetByFilterAsync(It.IsAny<Expression<Func<ReadOnlyEntityConcrete, bool>>>(), request, false, default), Times.Once);
    }

    /// <summary>
    /// GetAsync throws exception when request is null
    /// </summary>
    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _readOnlyForCollectionRequestHandler.GetAsync<RequestForDataObj>(null));
    }
}