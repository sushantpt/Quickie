using Quickie.DataHandlers.Writeonly.Definition;

namespace Quickie.DataHandlers.Writeonly.Implementation;

/// <summary>
/// Abstract class serving as write only data handler (Repository layer).
/// </summary>
/// <param name="dbContext">Db context</param>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDbContext">Db context</typeparam>
public abstract class WriteOnlyDataHandler<TEntity, TDbContext>(TDbContext dbContext) : IWriteOnlyDataHandler<TEntity> where TEntity : WriteOnlyEntity where TDbContext : DbContext
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="item">Data requested to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TEntity>> CreateItemAsync(TEntity item, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dbContext);
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        var response = new ResponseObj<TEntity>();
        try
        {
            await dbContext.Set<TEntity>().AddAsync(item, (CancellationToken)cancellationToken);
            await dbContext.SaveChangesAsync();
            response.IsSuccess = true;
            response.Data = item;
            response.Message = "Item created successfully.";
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occured while creating the data." : e.Message;
            response.Data = item;
        }

        return response;
    }

    /// <summary>
    /// Collection of data create request.
    /// </summary>
    /// <param name="items">Collection of data requested to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TEntity>>> CreateItemsAsync(ICollection<TEntity> items, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(dbContext);
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        
        var response = new ResponseObj<ICollection<TEntity>>();
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync((CancellationToken)cancellationToken);
            await dbContext.Set<TEntity>().AddRangeAsync(items, (CancellationToken)cancellationToken);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            response.IsSuccess = true;
            response.Data = items;
            response.Message = "Items created successfully.";
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occured while creating the data." : e.Message;
            response.Data = items;
        }
        return response;
    }
}