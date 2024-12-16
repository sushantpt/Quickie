namespace Quickie.Apis.Readonly;

/// <summary>
/// Abstract base class for read-only (GET) operations on data collections, utilizing a request handler to manage read operations.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object.</typeparam>
/// <typeparam name="TRequestHandler">The type of the request handler that processes the read-only operations.</typeparam>
/// <typeparam name="TRequestForDataModel">The type to provide request for collection of data.</typeparam>
/// <typeparam name="TIdType">Type of identifier (string, int, double, Guid, ...)</typeparam>
public class ReadOnlyCollectionController<TViewModel, TRequestHandler, TRequestForDataModel, TIdType>(TRequestHandler requestHandler) : QuickieBaseApi 
    where TViewModel : ReadOnlyDto where TRequestHandler : IReadOnlyCollectionRequestHandler<TViewModel, TIdType> where TRequestForDataModel : RequestForDataObj where TIdType : IComparable
{
    private readonly TRequestHandler _requestHandler = requestHandler;
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;

    /// <summary>
    /// GET request based on some request.
    /// </summary>
    /// <param name="request">Data filtration and pagination request object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated data with appropriate HTTP status codes.</returns>
    [HttpGet("items")]
    public async Task<ActionResult<PaginatedDataObj<TViewModel>>> GetAsync([FromQuery] TRequestForDataModel request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var data = await _requestHandler.GetAsync(request, cancellationToken);
                if (data.Items != null && data.Items.Any() && data.Total > 0)
                {
                    return Ok(data);
                }

                return NotFound(new
                {
                    Message = "No items found matching the query parameters."
                });
            }

            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(new
            {
                Message = $"Invalid request: {errors}"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Message = _options.ShowCustomErrorMessage ? "Failed to get data." : ex.Message
            });
        }
    }
}