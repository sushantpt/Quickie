namespace Quickie.DataHandlers.Readonly.Definition;

/// <summary>
/// Read-only data handler (Repository).
/// </summary>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TIdType">Type of id (int, Guid, string, ...)</typeparam>
public interface IReadOnlyDataHandler<TEntity, in TIdType> where TEntity : ReadOnlyEntity
{
    /// <summary>
    /// Get data by id. (ID must be primary key or composite key.)
    /// </summary>
    /// <param name="id">Unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Entity object.</returns>
    Task<TEntity?> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default);
}