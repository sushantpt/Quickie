namespace Quickie.Handlers.Readonly.Implementation;

/// <summary>
/// Abstract class serving as read only (collection) request handler.
/// </summary>
/// <param name="readOnlyCollectionDataHandler">Data handler (Repository layer)</param>
/// <typeparam name="TViewModel">View model or dto</typeparam>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDataHandler">Data handler (Repository layer)</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class ReadOnlyCollectionRequestHandler<TViewModel, TEntity, TDataHandler, TIdType>(IReadOnlyCollectionDataHandler<TEntity, TIdType> readOnlyCollectionDataHandler) 
    : IReadOnlyCollectionRequestHandler<TViewModel, TIdType> where TViewModel : ReadOnlyDto where TEntity : ReadOnlyEntity where TDataHandler : IReadOnlyCollectionDataHandler<TEntity, TIdType> where TIdType : IComparable
{
    /// <summary>
    /// Get data.
    /// </summary>
    /// <param name="request">Data request filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TRequestModel">Type of data request filter</typeparam>
    /// <returns></returns>
    public async Task<PaginatedDataObj<TViewModel>> GetAsync<TRequestModel>(TRequestModel? request, CancellationToken? cancellationToken = default) where TRequestModel : RequestForDataObj
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var response = new PaginatedDataObj<TViewModel>();
        Expression<Func<TEntity, bool>> condition = x => true;
        var data = await readOnlyCollectionDataHandler.GetByFilterAsync(condition, request, false, cancellationToken);
        var pagedData = MapToDto(data?.Items?.ToList()!);
        response.Items = pagedData;
        response.Total = data?.Total ?? 0;
        
        return response;
    }

    /// <summary>
    /// Mapping profile. Map from collection of Entity type to your collection of data transfer object.
    /// </summary>
    /// <param name="entity">Collection of entity object.</param>
    /// <returns>Mapped from collection of Entity, a collection data transfer object.</returns>
    protected abstract ICollection<TViewModel> MapToDto(ICollection<TEntity> entity);
}