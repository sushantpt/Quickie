namespace Quickie.Handlers.Readonly.Definition;

/// <summary>
/// Abstract base class for handling read-only requests, processing operations for the specified view model type.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object for read-only operations.</typeparam>
/// <typeparam name="TIdType">Type of id (int, Guid, string, ...)</typeparam>
public interface IReadOnlyRequestHandler<TViewModel, in TIdType> where TViewModel : ReadOnlyDto where TIdType : IComparable
{
    /// <summary>
    /// Get exact data (single) by id. 
    /// </summary>
    /// <param name="id">Identifier of data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Single data wrapped by response dto.</returns>
    Task<ResponseObj<TViewModel>> GetByIdAsync(TIdType id, CancellationToken? cancellationToken);
}