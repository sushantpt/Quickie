namespace Quickie.Handlers.Editonly.Implementation;

/// <summary>
/// Abstract class serving as edit only request handler.
/// </summary>
/// <param name="dataHandler">Data handler (Repository layer)</param>
/// <typeparam name="TViewModel">View model or dto</typeparam>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDataHandler">Data handler (Repository layer)</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class EditOnlyRequestHandler<TViewModel, TEntity, TDataHandler, TIdType>(IEditOnlyDataHandler<TEntity, TIdType> dataHandler) : IEditOnlyRequestHandler<TViewModel, TIdType> where TViewModel : EditOnlyDto where TEntity : EditOnlyEntity where TDataHandler : IEditOnlyDataHandler<TEntity, TIdType> where TIdType : IComparable
{
    /// <summary>
    /// Update request.
    /// </summary>
    /// <param name="id">Identifier of the entity.</param>
    /// <param name="requestModel">Requested data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TViewModel>> EditAsync(TIdType id, TViewModel requestModel, CancellationToken? cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(id));
        ArgumentNullException.ThrowIfNull(requestModel, nameof(requestModel));
        if (typeof(TIdType) == typeof(int) && id is < 1)
        {
            throw new ArgumentException("Id must be a positive integer.");
        }

        var response = new ResponseObj<TViewModel>();
        var dataToUpdate = MapToEntity(requestModel);
        var update = await dataHandler.EditAsync(id, dataToUpdate, cancellationToken);
        if (!update.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = update.Message;
            response.Data = null!;
            return response;
        }
        
        response.IsSuccess = true;
        response.Message = update.Message;
        response.Data = MapToDto(update.Data);

        return response;
    }

    /// <summary>
    /// Update collection of data.
    /// </summary>
    /// <param name="requestModels">Collection of requested data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TViewModel>>> EditCollectionAsync(ICollection<TViewModel> requestModels, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestModels, nameof(requestModels));
        
        var response = new ResponseObj<ICollection<TViewModel>>();
        var collectionToUpdate = MapToCollectionOfEntity(requestModels);
        var update = await dataHandler.EditCollectionAsync(collectionToUpdate, cancellationToken);
        if (!update.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = update.Message;
            response.Data = null!;
            return response;
        }
        
        response.IsSuccess = true;
        response.Message = update.Message;
        response.Data = MapToCollectionOfDto(update.Data);
        
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