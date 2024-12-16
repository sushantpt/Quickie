using Microsoft.EntityFrameworkCore;
using Moq;
using specification.Helpers.DataHandlers;
using specification.Helpers.Entities;

namespace specification.DataHandlers;

/// <summary>
/// Test for readonly data handler.
/// </summary>
public class ReadOnlyDataHandlerTests
{
    private readonly Mock<DbSet<ReadOnlyEntityConcrete>> _mockDbSet;
    private readonly ReadOnlyDataHandlerConcrete _handler;

    public ReadOnlyDataHandlerTests()
    {
        _mockDbSet = new Mock<DbSet<ReadOnlyEntityConcrete>>();
        Mock<DbContext> mockDbContext = new();

        mockDbContext.Setup(x => x.Set<ReadOnlyEntityConcrete>()).Returns(_mockDbSet.Object);

        _handler = new ReadOnlyDataHandlerConcrete(mockDbContext.Object);
    }

    /// <summary>
    /// GetByIdAsync returns data when data exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenIdExists()
    {
        var entity = new ReadOnlyEntityConcrete();

        _mockDbSet.Setup(x => x.FindAsync(1, CancellationToken.None)).ReturnsAsync(entity);

        var result = await _handler.GetByIdAsync(1);

        Assert.NotNull(result);
    }

    /// <summary>
    /// GetByIdAsync returns null when data does not exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
    {
        _mockDbSet.Setup(x => x.FindAsync(2, CancellationToken.None)).ReturnsAsync((ReadOnlyEntityConcrete?)null);

        var result = await _handler.GetByIdAsync(2);

        Assert.Null(result);
    }

    /// <summary>
    /// GetByIdAsync throws exception when passed dbContext is null.
    /// </summary>
    [Fact]
    public void GetByIdAsync_ShouldThrowException_WhenContextIsNull() => Assert.Throws<NullReferenceException>(() => new ReadOnlyDataHandlerConcrete(null!));
    
    /// <summary>
    /// GetByIdAsync throws exception when cancellation is requested.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenCancellationRequested()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        Assert.True(cancellationTokenSource.Token.IsCancellationRequested);
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await _handler.GetByIdAsync(1, cancellationTokenSource.Token));
    }
}