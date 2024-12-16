namespace Quickie.Apis.Crud;

/// <summary>
/// Base class providing CRUD apis.
/// </summary>
/// <param name="requestHandler">Request handler (Service layer)</param>
/// <typeparam name="TViewModel">Dto.</typeparam>
/// <typeparam name="TRequestHandler">Request handler (Service layer).</typeparam>
/// <typeparam name="TIdType">Type of your primary key.</typeparam>
public class CrudController<TViewModel, TRequestHandler, TIdType>(TRequestHandler requestHandler) : QuickieBaseApi where TViewModel : CrudDto where TRequestHandler : ICrudRequestHandler<TViewModel, TIdType> where TIdType : IComparable
{
    private readonly TRequestHandler _requestHandler = requestHandler;
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    /// <summary>
    /// Create data.
    /// </summary>
    /// <param name="idempotencyKey">Idempotency key</param>
    /// <param name="request">Requested data to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 201 Created, or 400 BadRequest.</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseObj<TViewModel>>> CreateAsync([FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] TViewModel request, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            if (ModelState.IsValid)
            {
                var response = await _requestHandler.CreateAsync(request, cancellationToken);
                if (response.IsSuccess)
                { 
                    return new ObjectResult(response)
                    {
                        StatusCode = StatusCodes.Status201Created
                    };
                }
                return BadRequest(response);
            }

            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(new ResponseObj<TViewModel> { IsSuccess = false, Message = $"Invalid requests': {string.Join(", ", errors)}", Data = null! });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<TViewModel>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to create request.", Data = request
            };
            return BadRequest(responseObj);
        }
    }
    
    /// <summary>
    /// Get data by id.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok, 404 NotFound, or 400 BadRequest.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseObj<TViewModel>>> GetByIdAsync([FromRoute] TIdType id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);
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
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to get data.", Data = null!
            };
            return BadRequest(responseObj);
        }
    }
    
    /// <summary>
    /// Update data.
    /// </summary>
    /// <param name="id">Identifier of data</param>
    /// <param name="request">Requested data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok, or 400 BadRequest.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseObj<TViewModel>>> UpdateAsync([FromRoute] TIdType id, [FromBody] TViewModel request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (typeof(TIdType) == typeof(int) && id is < 1)
            {
                return BadRequest();
            }
            ArgumentNullException.ThrowIfNull(id);
            if (ModelState.IsValid)
            {
                var update = await _requestHandler.UpdateAsync(id, request, cancellationToken);
                if (update.IsSuccess)
                {
                    return Ok(update);
                } 
                return BadRequest(update);
            }
            
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(new ResponseObj<TViewModel> 
            { 
                IsSuccess = false, 
                Message = $"Invalid request: {errors}", 
                Data = null! 
            });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<TViewModel>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? ex.Message : "Failed to update request.", Data = null!
            };
            return BadRequest(responseObj);
        }
    }
    
    /// <summary>
    /// Delete data
    /// </summary>
    /// <param name="id">Identifier of data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 204 NoContent or 400 BadRequest.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseObj>> DeleteAsync([FromRoute] TIdType id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);
            if (typeof(TIdType) == typeof(int) && id is < 1)
            {
                return BadRequest();
            }
            var delete = await _requestHandler.DeleteAsync(id, cancellationToken);
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