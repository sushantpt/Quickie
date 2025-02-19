namespace Quickie.DataHandlers.Writeonly.Definition;

/// <summary>
/// Defines write-only data operations.
/// </summary>
/// <typeparam name="TEntity">Type of entity</typeparam>
public interface IWriteOnlyDataHandler<TEntity> where TEntity : WriteOnlyEntity
{
    /// <summary>
    /// Create an item
    /// </summary>
    /// <param name="item">Data to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<TEntity>> CreateItemAsync(TEntity item, CancellationToken? cancellationToken); 
    
    /// <summary>
    /// Create collection of data
    /// </summary>
    /// <param name="items">Collection of data or items to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<ICollection<TEntity>>> CreateItemsAsync(ICollection<TEntity> items, CancellationToken? cancellationToken); 
}