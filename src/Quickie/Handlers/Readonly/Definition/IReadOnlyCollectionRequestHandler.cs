namespace Quickie.Handlers.Readonly.Definition;

/// <summary>
/// Abstract base class for handling collections of read-only requests, processing operations for the specified view model type.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object for read-only operations.</typeparam>
/// <typeparam name="TIdType">Type of id (int, Guid, string, ...)</typeparam>
public interface IReadOnlyCollectionRequestHandler<TViewModel, in TIdType> where TViewModel : ReadOnlyDto where TIdType : IComparable
{
    /// <summary>
    /// Get collection of data by provided request model.
    /// </summary>
    /// <param name="request">Data request model of type RequestForDataDto.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of data wrapped by response dto.</returns>
    Task<PaginatedDataObj<TViewModel>> GetAsync<TRequestModel>(TRequestModel? request, CancellationToken? cancellationToken) where TRequestModel : RequestForDataObj;
}