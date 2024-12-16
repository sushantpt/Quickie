using Quickie.DataHandlers.Writeonly.Definition;

namespace Quickie.Handlers.Writeonly.Implementation;

/// <summary>
/// Abstract class serving as write only request handler.
/// </summary>
/// <param name="dataHandler"></param>
/// <typeparam name="TViewModel"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDataHandler"></typeparam>
public abstract class WriteOnlyRequestHandler<TViewModel, TEntity, TDataHandler>(IWriteOnlyDataHandler<TEntity> dataHandler) : IWriteOnlyRequestHandler<TViewModel> where TViewModel : WriteOnlyDto where TEntity : WriteOnlyEntity where TDataHandler : IWriteOnlyDataHandler<TEntity>
{
    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="item">Data requested to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TViewModel>> CreateItemAsync(TViewModel item, CancellationToken? cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        var response = new ResponseObj<TViewModel>();
        var dataToAdd = MapToEntity(item);
        var add = await dataHandler.CreateItemAsync(dataToAdd, cancellationToken);
        if (!add.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = add.Message;
            response.Data = item;
            return response;
        }
        response.IsSuccess = true;
        response.Data = MapToDto(add.Data);
        return response;
    }

    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="items">Collection of data requested to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TViewModel>>> CreateItemsAsync(ICollection<TViewModel> items, CancellationToken? cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(items));
        var response = new ResponseObj<ICollection<TViewModel>>();
        var dataToAdd = MapToCollectionOfEntity(items);
        var add = await dataHandler.CreateItemsAsync(dataToAdd, cancellationToken);
        if (!add.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = add.Message;
            response.Data = items;
            return response;
        }
        response.IsSuccess = true;
        response.Data = MapToCollectionOfDto(add.Data);
        return response;
    }
    
    /// <summary>
    /// Maps a <typeparamref name="TViewModel"/> to a <typeparamref name="TEntity"/> entity.
    /// </summary>
    /// <param name="requestModel">The view model/dto to map to an entity</param>
    /// <returns>Entity.</returns>
    protected abstract TEntity MapToEntity(TViewModel requestModel);
    
    /// <summary>
    /// Maps a <typeparamref name="TEntity"/> entity to a <typeparamref name="TViewModel"/> view model/dto.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <returns>View model/dto.</returns>
    protected abstract TViewModel MapToDto(TEntity entity);
    
    /// <summary>
    /// Maps a collection of <typeparamref name="TViewModel"/> to a collection of <typeparamref name="TEntity"/> entities.
    /// </summary>
    /// <param name="requestModels">Collection of view model/dto</param>
    /// <returns>Collection of entity.</returns>
    protected abstract ICollection<TEntity> MapToCollectionOfEntity(ICollection<TViewModel> requestModels);
    
    /// <summary>
    /// Maps a collection of <typeparamref name="TEntity"/> entities to a collection of <typeparamref name="TViewModel"/> view models.
    /// </summary>
    /// <param name="entities">Collection of entity.</param>
    /// <returns>Collection of view model/dto.</returns>
    protected abstract ICollection<TViewModel> MapToCollectionOfDto(ICollection<TEntity> entities);
}