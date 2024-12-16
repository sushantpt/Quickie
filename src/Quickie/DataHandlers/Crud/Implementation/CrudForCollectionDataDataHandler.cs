namespace Quickie.DataHandlers.Crud.Implementation;

/// <summary>
/// Abstract class serving as CRUD for collection request handler.
/// </summary>
/// <param name="dbContext">Db context</param>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
/// <typeparam name="TDbContext">Db context</typeparam>
public abstract class CrudForCollectionDataDataHandler<TEntity, TIdType, TDbContext>(TDbContext dbContext) : ICrudForCollectionDataHandler<TEntity, TIdType> where TEntity : CrudEntity where TIdType : IComparable where TDbContext : DbContext
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="entities">Collection of data requested to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var response = new ResponseObj<ICollection<TEntity>>();
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync((CancellationToken)cancellationToken);
            await dbContext.AddRangeAsync(entities, (CancellationToken)cancellationToken);
            await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);
            await transaction.CommitAsync((CancellationToken)cancellationToken);
            response.Data = entities;
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occurred while creating the data." : ex.Message;
            response.Data = entities;
        }
        return response;
    }

    /// <summary>
    /// Update request.
    /// </summary>
    /// <param name="entities">Requested collection data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TEntity>>> UpdateRangeAsync(ICollection<TEntity> entities, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var response = new ResponseObj<ICollection<TEntity>>();
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync((CancellationToken)cancellationToken);
            dbContext.UpdateRange(entities);
            var updatedEntities = await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);
            await transaction.CommitAsync((CancellationToken)cancellationToken);
            response.IsSuccess = updatedEntities > 0;
            response.Data = entities;
            response.Message = updatedEntities > 0 ? $"{entities.Count} entities updated successfully." : "No changes were made. Update failed.";
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occurred while updating the data." : ex.Message;
            response.Data = entities;
        } 
        return response;
    }

    /// <summary>
    /// Delete request.
    /// </summary>
    /// <param name="ids">Identifiers of the data to remove.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj> DeleteRangeAsync(ICollection<TIdType> ids, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        var response = new ResponseObj();
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync((CancellationToken)cancellationToken);
            dbContext.RemoveRange(ids);
            await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);
            await transaction.CommitAsync((CancellationToken)cancellationToken);
            response.IsSuccess = true;
            response.Message = $"{ids.Count} data removed successfully.";
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occurred while deleting the data." : ex.Message;
        }
        return response;
    }

    /// <summary>
    /// Get data.
    /// </summary>
    /// <param name="request">Data request filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<PaginatedDataObj<TEntity>>> GetAllAsync(RequestForDataObj request, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var response = new ResponseObj<PaginatedDataObj<TEntity>>();
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        try
        {
            var data  = await dbContext.Set<TEntity>()
                .AsNoTracking()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync((CancellationToken)cancellationToken);
            response.Data = new PaginatedDataObj<TEntity>
            {
                Items = data,
                Total = await dbContext.Set<TEntity>().AsNoTracking().CountAsync((CancellationToken)cancellationToken),
            };
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occurred while retrieving the data." : ex.Message;
            response.Data = new PaginatedDataObj<TEntity>
            {
                Items = [],
                Total = 0
            };
        }
        return response;
    }

    /// <summary>
    /// Count.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<int>> CountAsync(CancellationToken? cancellationToken = default)
    {
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        try
        {
            var count = await dbContext.Set<TEntity>().AsNoTracking().CountAsync((CancellationToken)cancellationToken);
            return new ResponseObj<int>
            {
                IsSuccess = true,
                Data = count,
                Message = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ResponseObj<int>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? "An error occurred while counting the data." : ex.Message,
                Data = 0
            };
        }
    }
}