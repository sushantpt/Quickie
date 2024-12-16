namespace Quickie.Handlers.Readonly.Implementation;

/// <summary>
/// Abstract base class for handling read-only requests, processing operations for the specified view model type.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object for read-only operations.</typeparam>
/// <typeparam name="TEntity">Entity.</typeparam>
/// <typeparam name="TDataHandler">Data handler (Repository)</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class ReadOnlyRequestHandler<TViewModel, TEntity, TDataHandler, TIdType>(IReadOnlyDataHandler<TEntity, TIdType> dataHandler) : IReadOnlyRequestHandler<TViewModel, TIdType> where TViewModel : ReadOnlyDto where TEntity : ReadOnlyEntity where TDataHandler : IReadOnlyDataHandler<TEntity, TIdType> where TIdType : IComparable
{
    /// <summary>
    /// Get data by id.
    /// </summary>
    /// <param name="id">Identifier (unique key identifying your entity)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ResponseObj<TViewModel>> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (typeof(TIdType) == typeof(int) && id is < 1)
        {
            throw new ArgumentException("Id must be a positive integer.");
        }
        
        var response = new ResponseObj<TViewModel>();
        var data = await dataHandler.GetByIdAsync(id, cancellationToken);
        if (data is not null)
        {
            response.Data = MapToDto(data);
            response.IsSuccess = true;
            response.Message = "Success.";
        }
        else
        {
            response.IsSuccess = false;
            response.Message = "Data not found.";
        }
        return response;
    }

    /// <summary>
    /// Mapping profile. Map from Entity type to your data transfer object.
    /// </summary>
    /// <param name="entity">Entity object.</param>
    /// <returns>Mapped from Entity, a data transfer object.</returns>
    protected abstract TViewModel MapToDto(TEntity entity);
}