namespace Quickie.DataHandlers.Editonly.Definition;

/// <summary>
/// Defines operations to update entities
/// </summary>
/// <typeparam name="TEntity">Data to update.</typeparam>
/// <typeparam name="TIdType">Type of id (int, Guid, string, ...)</typeparam>
public interface IEditOnlyDataHandler<TEntity, in TIdType> where TEntity : EditOnlyEntity where TIdType : IComparable
{
    /// <summary>
    /// Update single data.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="requestModel">Data to be updated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<ResponseObj<TEntity>> EditAsync(TIdType id, TEntity requestModel, CancellationToken? cancellationToken);

    /// <summary>
    /// Update collection of data (bulk update).
    /// </summary>
    /// <param name="requestModels">Collection of entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<ResponseObj<ICollection<TEntity>>> EditCollectionAsync(ICollection<TEntity> requestModels, CancellationToken? cancellationToken);
}