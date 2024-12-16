namespace Quickie.Handlers.Crud.Implementation;

/// <summary>
/// Abstract class serving as CRUD request handler.
/// </summary>
/// <param name="dataHandler">Data handler (repository layer)</param>
/// <typeparam name="TViewModel">View model or dto</typeparam>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDataHandler">Data handler (repository layer)</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class CrudRequestHandler<TViewModel, TEntity, TDataHandler, TIdType>(ICrudDataHandler<TEntity, TIdType> dataHandler) : ICrudRequestHandler<TViewModel, TIdType> where TViewModel : CrudDto where TEntity : CrudEntity where TDataHandler : ICrudDataHandler<TEntity, TIdType> where TIdType : IComparable
{
    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="request">Data requested to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TViewModel>> CreateAsync(TViewModel request, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken ??= CancellationToken.None;
        var dataToCreate = MapToEntity(request);
        var create = await dataHandler.CreateAsync(dataToCreate, cancellationToken);
        return new ResponseObj<TViewModel>
        {
            IsSuccess = create.IsSuccess,
            Message = create.Message,
            Data = create is { Data: not null } ? MapToDto(create.Data) : request
        };
    }

    /// <summary>
    /// Get data by providing an identifier
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ResponseObj<TViewModel>> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (typeof(TIdType) == typeof(int) && id is < 1)
        {
            throw new ArgumentException("Id must be a positive integer.");
        }
        cancellationToken ??= CancellationToken.None;
        var data = await dataHandler.GetByIdAsync(id, cancellationToken);
        return new ResponseObj<TViewModel>
        {
            IsSuccess = data.IsSuccess,
            Message = data.Message,
            Data = data.Data is { } entity ? MapToDto(entity) : null!
        };
    }

    /// <summary>
    /// Update request.
    /// </summary>
    /// <param name="id">Requested data identifier.</param>
    /// <param name="request">Requested data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TViewModel>> UpdateAsync(TIdType id, TViewModel request, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(request);
        if (typeof(TIdType) == typeof(int) && id is < 1)
        {
            throw new ArgumentException("Id must be a positive integer.");
        }
        cancellationToken ??= CancellationToken.None;
        var dataToUpdate = MapToEntity(request);
        var update = await dataHandler.UpdateAsync(id, dataToUpdate, cancellationToken);
        return new ResponseObj<TViewModel>
        {
            IsSuccess = update.IsSuccess,
            Message = update.Message,
            Data = update.Data is { } entity ? MapToDto(entity) : request
        };
    }

    /// <summary>
    /// Delete request.
    /// </summary>
    /// <param name="id">Identifier of the data to remove.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ResponseObj> DeleteAsync(TIdType id, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (typeof(TIdType) == typeof(int) && id is < 1)
        {
            throw new ArgumentException("Id must be a positive integer.");
        }
        cancellationToken ??= CancellationToken.None;
        return await dataHandler.DeleteAsync(id, cancellationToken);
    }
    
    /// <summary>
    /// Maps a <typeparamref name="TViewModel"/> to a <typeparamref name="TEntity"/> entity.
    /// </summary>
    /// <param name="request">Dto</param>
    /// <returns>Entity</returns>
    protected abstract TEntity MapToEntity(TViewModel request);
    
    /// <summary>
    /// Maps a <typeparamref name="TEntity"/> to a <typeparamref name="TViewModel"/> dto.
    /// </summary>
    /// <param name="request">Entity</param>
    /// <returns>Dto</returns>
    protected abstract TViewModel MapToDto(TEntity request);
}