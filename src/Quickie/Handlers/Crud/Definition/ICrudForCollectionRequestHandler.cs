namespace Quickie.Handlers.Crud.Definition;

/// <summary>
/// Abstract base class for handling collections of CRUD requests (or CRUD for collection object).
/// </summary>
/// <typeparam name="TViewModel">Type of view model that represents the data transfer object for collection of CRUD operations.</typeparam>
/// <typeparam name="TIdType"></typeparam>
public interface ICrudForCollectionRequestHandler<TViewModel, TIdType> where TViewModel : CrudDto where TIdType : IComparable
{
    /// <summary>
    /// Create data in bulk.
    /// </summary>
    /// <param name="entities">Requested collection of data to be created</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<ICollection<TViewModel>>> CreateRangeAsync(ICollection<TViewModel> entities, CancellationToken? cancellationToken = default);
    
    /// <summary>
    /// Update requested data
    /// </summary>
    /// <param name="entities">Collection of data requested to be updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<ICollection<TViewModel>>> UpdateRangeAsync(ICollection<TViewModel> entities, CancellationToken? cancellationToken = default);

    /// <summary>
    /// Removes data.
    /// </summary>
    /// <param name="ids">Data identifier to be removed.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj> DeleteRangeAsync(ICollection<TIdType> ids, CancellationToken? cancellationToken = default);

    /// <summary>
    /// Get all data
    /// </summary>
    /// <param name="request">Request object to filter data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<PaginatedDataObj<TViewModel>>> GetAllAsync(RequestForDataObj request, CancellationToken? cancellationToken = default);

    /// <summary>
    /// Get total count of data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<int>> CountAsync(CancellationToken? cancellationToken = default);
}