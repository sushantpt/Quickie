namespace Quickie.Apis.Writeonly;

public class WriteOnlyController<TViewModel, TRequestHandler>(IWriteOnlyRequestHandler<TViewModel> requestHandler) : QuickieBaseApi where TViewModel : WriteOnlyDto where TRequestHandler : IWriteOnlyRequestHandler<TViewModel>
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;

    /// <summary>
    /// Create a single item.
    /// </summary>
    /// <param name="idempotencyKey">Idempotency key.</param>
    /// <param name="request">Data to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure of the operation.</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseObj<TViewModel>>> CreateItemAsync([FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey,  [FromBody]TViewModel request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = $"Invalid request: {errors}",
                    Data = null!
                });
            }

            var result = await requestHandler.CreateItemAsync(request, cancellationToken);
            if (result.IsSuccess)
            {
                return new ObjectResult(result)
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<TViewModel>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? "Failed to create request." : ex.Message,
                Data = null!
            };
            return BadRequest(responseObj);
        }
    }
    
    /// <summary>
    /// Create multiple items in bulk.
    /// </summary>
    /// <param name="idempotencyKey">idempotency key</param>
    /// <param name="requests">Requested data to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure of the operation.</returns>
    [HttpPost("items")]
    public async Task<ActionResult<ResponseObj<ICollection<TViewModel>>>> CreateItemsAsync([FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] ICollection<TViewModel> requests, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new ResponseObj<ICollection<TViewModel>>
                {
                    IsSuccess = false,
                    Message = $"Invalid request: {errors}",
                    Data = null!
                });
            }

            var result = await requestHandler.CreateItemsAsync(requests, cancellationToken);
            if (result.IsSuccess)
            {
                return new ObjectResult(result)
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? "An error occurred while creating the items." : ex.Message,
                Data = null!
            };
            return BadRequest(responseObj);
        }
    }
}