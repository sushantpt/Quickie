namespace Quickie.DataHandlers.Readonly.Definition;

/// <summary>
/// Defines read-only data access operations for collections of entities. Provides methods to retrieve data with optional filtering and pagination.
/// </summary>
/// <typeparam name="TEntity">The type of entity to do read-only operations.</typeparam>
/// <typeparam name="TIdType">Type of id (int, Guid, string, ...)</typeparam>
public interface IReadOnlyCollectionDataHandler<TEntity, in TIdType> where TEntity : ReadOnlyEntity where TIdType : IComparable
{
    /// <summary>
    /// Get a paginated collection of entities that match the specified filter.
    /// </summary>
    /// <param name="filter">A predicate function to define your condition.</param>
    /// <param name="request">The pagination request object.</param>
    /// <param name="track">A flag indicating whether to track changes for the retrieved entities. Set to <c>false</c> to disable change tracking for read-only queries. Default is <c>false</c>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated data.</returns>
    Task<PaginatedDataObj<TEntity>> GetByFilterAsync<TRequestModel>(Expression<Func<TEntity, bool>>? filter, TRequestModel? request, bool? track = false, CancellationToken? cancellationToken = default) where TRequestModel : RequestForDataObj;
}