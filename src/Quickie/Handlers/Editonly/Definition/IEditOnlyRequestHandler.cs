namespace Quickie.Handlers.Editonly.Definition;

/// <summary>
/// Abstract base class for handling update requests, processing operations for the specified view model type.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object to be updated.</typeparam>
/// <typeparam name="TIdType">Type of id (int, string, guid, ...)</typeparam>
public interface IEditOnlyRequestHandler<TViewModel, in TIdType> where TViewModel : EditOnlyDto where TIdType : IComparable
{
    /// <summary>
    /// Update single entity.
    /// </summary>
    /// <param name="id">Identifier of the entity.</param>
    /// <param name="requestModel">Data to be updated.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<ResponseObj<TViewModel>> EditAsync(TIdType id, TViewModel requestModel, CancellationToken? cancellationToken);
    
    /// <summary>
    /// Update collection of entity. (Bulk)
    /// </summary>
    /// <param name="requestModels">Bulk data to be updated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<ResponseObj<ICollection<TViewModel>>> EditCollectionAsync(ICollection<TViewModel> requestModels, CancellationToken? cancellationToken);
}