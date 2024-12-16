namespace Quickie.DataHandlers.Readonly.Implementation;

/// <summary>
/// Abstract class serving as read only (collection) data handler (Repository layer).
/// </summary>
/// <param name="context">Db Context</param>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDbContext">Db Context</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class ReadOnlyCollectionDataHandler<TEntity, TDbContext, TIdType>(DbContext context) : IReadOnlyCollectionDataHandler<TEntity, TIdType> where TEntity : ReadOnlyEntity where TDbContext : DbContext where TIdType : IComparable
{
    private readonly TDbContext _context = context as TDbContext ?? throw new NullReferenceException("DbContext is cannot be null.");

    /// <summary>
    /// Get filtered collection of data.
    /// </summary>
    /// <param name="filter">Data request filter</param>
    /// <param name="request">Data request filter</param>
    /// <param name="track">Track entity? Default is false</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TRequestModel">Type of data request filter</typeparam>
    /// <returns></returns>
    public async Task<PaginatedDataObj<TEntity>> GetByFilterAsync<TRequestModel>(Expression<Func<TEntity, bool>>? filter, TRequestModel? request, bool? track = false, CancellationToken? cancellationToken = default) where TRequestModel : RequestForDataObj
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(filter));
        request ??= (TRequestModel)new RequestForDataObj();
        
        var response = new PaginatedDataObj<TEntity>();
        
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        
        var query = _context.Set<TEntity>().Where(filter ?? (x => true))
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);
        query = track is null or false ? query.AsNoTracking() : query;
        var data = await query.ToListAsync((CancellationToken)cancellationToken);
        response.Items = data;
        response.Total = await _context.Set<TEntity>().Where(filter ?? (x => true)).AsNoTracking().CountAsync((CancellationToken)cancellationToken);
        return response;
    }
}