namespace Quickie.Handlers.Crud.Implementation;

/// <summary>
/// Abstract class serving as CRUD for collection request handler.
/// </summary>
/// <param name="dataHandler">Data</param>
/// <typeparam name="TViewModel">View model or dto</typeparam>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDataHandler">Data handler (repository layer)</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class CrudForCollectionRequestHandler<TViewModel, TEntity, TDataHandler, TIdType>(TDataHandler dataHandler) : ICrudForCollectionRequestHandler<TViewModel, TIdType> where TViewModel : CrudDto where TEntity : CrudEntity where TDataHandler : ICrudForCollectionDataHandler<TEntity, TIdType> where TIdType : IComparable
{
    private TDataHandler _dataHandler = dataHandler;

    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="entities">Collection of data requested to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TViewModel>>> CreateRangeAsync(ICollection<TViewModel> entities, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        cancellationToken ??= CancellationToken.None;
        var dataToCreate = MapToCollectionOfEntity(entities);
        
        var create = await _dataHandler.CreateRangeAsync(dataToCreate, cancellationToken);
        if (!create.IsSuccess) return new ResponseObj<ICollection<TViewModel>> { IsSuccess = false, Message = create.Message, Data = null! };
        
        var createdEntities = MapToCollectionOfDto(create.Data);
        return new ResponseObj<ICollection<TViewModel>>
        {
            IsSuccess = true,
            Data = createdEntities,
            Message = create.Message
        };
    }

    /// <summary>
    /// Update request.
    /// </summary>
    /// <param name="entities">Requested collection data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TViewModel>>> UpdateRangeAsync(ICollection<TViewModel> entities, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var dataToUpdate = MapToCollectionOfEntity(entities);
        var update = await _dataHandler.UpdateRangeAsync(dataToUpdate, cancellationToken);
        if (!update.IsSuccess) return new ResponseObj<ICollection<TViewModel>> { IsSuccess = false, Message = update.Message, Data = entities };
        var updatedEntities = MapToCollectionOfDto(update.Data);
        return new ResponseObj<ICollection<TViewModel>>
        {
            IsSuccess = true,
            Message = update.Message,
            Data = updatedEntities
        };
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
        return await _dataHandler.DeleteRangeAsync(ids, cancellationToken);
    }

    /// <summary>
    /// Get data.
    /// </summary>
    /// <param name="request">Data request filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<PaginatedDataObj<TViewModel>>> GetAllAsync(RequestForDataObj request, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var response = new ResponseObj<PaginatedDataObj<TViewModel>>()
        {
            Data = new PaginatedDataObj<TViewModel>
            {
                Items = new List<TViewModel>()
            }
        };
        var data = await _dataHandler.GetAllAsync(request, cancellationToken);
        if (data.IsSuccess)
        {
            var items = MapToCollectionOfDto(data.Data.Items.ToList());
            response.Data.Items = items;
            response.Data.Total = data.Data.Total;
        }
        response.IsSuccess = data.IsSuccess;
        response.Message = data.Message;
        return response;
    }

    /// <summary>
    /// Count.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<int>> CountAsync(CancellationToken? cancellationToken = default) => await _dataHandler.CountAsync(cancellationToken);
    
    /// <summary>
    /// Maps a collection of <typeparamref name="TEntity"/> to a collection of <typeparamref name="TViewModel"/> dto.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected abstract ICollection<TViewModel> MapToCollectionOfDto(ICollection<TEntity> entity);

    /// <summary>
    /// Maps a collection of <typeparamref name="TViewModel"/> to a collection of <typeparamref name="TEntity"/> entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected abstract ICollection<TEntity> MapToCollectionOfEntity(ICollection<TViewModel> entity);
}