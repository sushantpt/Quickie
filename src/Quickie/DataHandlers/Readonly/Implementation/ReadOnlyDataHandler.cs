namespace Quickie.DataHandlers.Readonly.Implementation;

/// <summary>
/// Abstract base class for handling read-only data requests.
/// </summary>
/// <param name="context">Db context</param>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDbContext">Db context</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class ReadOnlyDataHandler<TEntity, TDbContext, TIdType>(DbContext context) : IReadOnlyDataHandler<TEntity, TIdType> where TEntity : ReadOnlyEntity where TDbContext : DbContext where TIdType : IComparable
{
    private readonly TDbContext _context = context as TDbContext ?? throw new NullReferenceException("DbContext cannot be null.");
    
    /// <summary>
    /// Get data by id.
    /// </summary>
    /// <param name="id">Identifier (unique key identifying your entity)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<TEntity?> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default)
    {
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        var find =  await _context.Set<TEntity>().FindAsync(id, cancellationToken);
        return find;
    }
}