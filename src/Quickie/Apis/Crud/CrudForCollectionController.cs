namespace Quickie.Apis.Crud;

/// <summary>
/// Base class providing CRUD apis in collection.
/// </summary>
/// <typeparam name="TViewModel">Dto</typeparam>
/// <typeparam name="TRequestHandler">Request handler (Service layer)</typeparam>
/// <typeparam name="TIdType">Type of your primary key.</typeparam>
public class CrudForCollectionController<TViewModel, TRequestHandler, TIdType>(TRequestHandler requestHandler) : QuickieBaseApi where TViewModel : CrudDto where TRequestHandler : ICrudForCollectionRequestHandler<TViewModel, TIdType> where TIdType : IComparable
{
    private readonly TRequestHandler _requestHandler = requestHandler;
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    /// <summary>
    /// Create collection of data.
    /// </summary>
    /// <param name="idempotencyKey">Idempotency key.</param>
    /// <param name="request">Requested data to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 201 Created, or 400 BadRequest.</returns>
    [HttpPost("items")]
    public async Task<ActionResult<ResponseObj<ICollection<TViewModel>>>> CreateAsync([FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] ICollection<TViewModel> request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var create = await _requestHandler.CreateRangeAsync(request, cancellationToken);
                if (create.IsSuccess)
                {
                    return new ObjectResult(create)
                    {
                        StatusCode = StatusCodes.Status201Created
                    };
                }
                return BadRequest(create);
            }

            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false, Message = $"Invalid requests': {string.Join(", ", errors)}", Data = null! 
            });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to create request.", Data = request
            };
            return BadRequest(responseObj);
        }
    }

    /// <summary>
    /// Update collection of data.
    /// </summary>
    /// <param name="request">Requested data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok, or 400 BadRequest.</returns>
    [HttpPut("items")]
    public async Task<ActionResult<ResponseObj<ICollection<TViewModel>>>> UpdateAsync(ICollection<TViewModel> request, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            if (ModelState.IsValid)
            {
                var update = await _requestHandler.UpdateRangeAsync(request, cancellationToken);
                if (update.IsSuccess)
                {
                    return Ok(update);
                } 
                return BadRequest(update);
            }
        
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false,
                Message = $"Invalid request: {errors}",
            });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to update request.", Data = null!
            };
            return BadRequest(responseObj);
        }
    }

    /// <summary>
    /// Get all data. (Paginated)
    /// </summary>
    /// <param name="request">Request object.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok, 404 NotFound, or 400 BadRequest.</returns>
    [HttpGet("items")]
    public async Task<ActionResult<ResponseObj<PaginatedDataObj<TViewModel>>>> GetAllAsync([FromBody] RequestForDataObj request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            if (ModelState.IsValid)
            {
                var get = await _requestHandler.GetAllAsync(request, cancellationToken);
                if (get.IsSuccess)
                {
                    return Ok(get);
                }
                return NotFound(get);
            }
        
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return  BadRequest(new ResponseObj<PaginatedDataObj<TViewModel>>
            {
                IsSuccess = false,
                Message = $"Invalid request: {errors}",
                Data = null!
            });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<PaginatedDataObj<TViewModel>>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to get data.", Data = null!
            };
            return BadRequest(responseObj);
        }
    }
    
    /// <summary>
    /// Total count of data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok or 400 BadRequest.</returns>
    [HttpGet("items/count")]
    public async Task<ActionResult<ResponseObj<int>>> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var count = await _requestHandler.CountAsync(cancellationToken);
                if (count.IsSuccess)
                {
                    return Ok(count);
                }

                return BadRequest(count);
            }
        
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(new ResponseObj<int>
            {
                IsSuccess = false,
                Message = $"Invalid request: {errors}",
                Data = 0
            });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<int>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to get data.", Data = 0
            };
            return BadRequest(responseObj);
        }
    }
    
    /// <summary>
    /// DELETE request for collection of data.
    /// </summary>
    /// <param name="ids">Collection of id.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 204 NoContent or 400 BadRequest.</returns>
    [HttpDelete("items")]
    public async Task<ActionResult<ResponseObj>> DeleteAsync(ICollection<TIdType> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids);
            var delete = await _requestHandler.DeleteRangeAsync(ids, cancellationToken);
            if (delete.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(delete);
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to update request."
            };
            return BadRequest(responseObj);
        }
    }
}