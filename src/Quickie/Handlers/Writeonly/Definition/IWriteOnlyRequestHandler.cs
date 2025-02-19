namespace Quickie.Handlers.Writeonly.Definition;

/// <summary>
/// Abstract base class for handling write only requests, processing operations for the specified view model type.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object for WRITE-only operations.</typeparam>
public interface IWriteOnlyRequestHandler<TViewModel> where TViewModel : WriteOnlyDto
{
    /// <summary>
    /// Create an item
    /// </summary>
    /// <param name="item">Data request model to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<ResponseObj<TViewModel>> CreateItemAsync(TViewModel item, CancellationToken? cancellationToken); 
    
    /// <summary>
    /// Create items (bulk)
    /// </summary>
    /// <param name="items">Data request model to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection response object</returns>
    Task<ResponseObj<ICollection<TViewModel>>> CreateItemsAsync(ICollection<TViewModel> items, CancellationToken? cancellationToken); 
}