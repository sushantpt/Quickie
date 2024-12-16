namespace Quickie.Apis.Readonly;

/// <summary>
/// Abstract base class for read-only (GETs) operations that utilize a request handler for managing read operations.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model that represents the data transfer object.</typeparam>
/// <typeparam name="TRequestHandler">The type of the request handler that processes the read-only operations.</typeparam>
/// <typeparam name="TIdType">Type of identifier (string, int, double, Guid, ...).</typeparam>
public class ReadOnlyController<TViewModel, TRequestHandler, TIdType>(TRequestHandler requestHandler) : QuickieBaseApi
    where TViewModel : ReadOnlyDto
    where TRequestHandler : IReadOnlyRequestHandler<TViewModel, TIdType> where TIdType : IComparable
{
    private readonly TRequestHandler _requestHandler = requestHandler;
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseObj<TViewModel>>> GetByIdAsync(TIdType id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(id));
            if (typeof(TIdType) == typeof(int) && id is < 1)
            {
                return BadRequest();
            }
            var data = await _requestHandler.GetByIdAsync(id, cancellationToken);
            if (data.IsSuccess)
            {
                return Ok(data);
            }

            return NotFound(data);
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<TViewModel>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? "Failed to get data." : ex.Message,
                Data = null!
            };
            return BadRequest(responseObj);
        }
    }
}