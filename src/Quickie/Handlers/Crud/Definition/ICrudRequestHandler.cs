namespace Quickie.Handlers.Crud.Definition;

/// <summary>
/// Abstract base class for CRUD operation. Contains method to: Create, Update, Read (GET), and Delete.
/// </summary>
/// <typeparam name="TViewModel"></typeparam>
/// <typeparam name="TIdType">Type of the identifier used in methods (string, int, Guid, ...)</typeparam>
public interface ICrudRequestHandler<TViewModel, in TIdType> where TViewModel : CrudDto where TIdType : IComparable
{
    /// <summary>
    /// Create data.
    /// </summary>
    /// <param name="request">Requested data to be created.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object.</returns>
    Task<ResponseObj<TViewModel>> CreateAsync(TViewModel request, CancellationToken? cancellationToken);
    
    /// <summary>
    /// Get data by id (unique identifier, a pk).
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object with data.</returns>
    Task<ResponseObj<TViewModel>> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default);

    /// <summary>
    /// Update requested data.
    /// </summary>
    /// <param name="id">Data identifier to update.</param>
    /// <param name="request">Data requested to be updated.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object with updated data.</returns>
    Task<ResponseObj<TViewModel>> UpdateAsync(TIdType id, TViewModel request, CancellationToken? cancellationToken = default);
    
    /// <summary>
    /// Removes data.
    /// </summary>
    /// <param name="id">Identifier of data to be removed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj> DeleteAsync(TIdType id, CancellationToken? cancellationToken = default);
}