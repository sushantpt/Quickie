namespace Quickie.Apis.Editonly;

/// <summary>
/// Base class providing Edit-Only APIs.
/// </summary>
/// <typeparam name="TViewModel">Dto</typeparam>
/// <typeparam name="TRequestHandler">Request handler (Service layer)</typeparam>
/// <typeparam name="TIdType">Type of your primary key</typeparam>
public class EditOnlyController<TViewModel, TRequestHandler, TIdType>(IEditOnlyRequestHandler<TViewModel, TIdType> requestHandler) : QuickieBaseApi where TViewModel : EditOnlyDto where TRequestHandler : IEditOnlyRequestHandler<TViewModel, TIdType> where TIdType : IComparable
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;

    /// <summary>
    /// Edit a single item.
    /// </summary>
    /// <param name="id">Identifier of the item</param>
    /// <param name="requestModel">Data for editing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok, or 400 BadRequest.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseObj<TViewModel>>> EditAsync([FromRoute] TIdType id, [FromBody] TViewModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(requestModel);
            if (typeof(TIdType) == typeof(int) && id is < 1)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var response = await requestHandler.EditAsync(id, requestModel, cancellationToken);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                return BadRequest(response);
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
                Message = _options.ShowCustomErrorMessage ? "Failed to update data." : ex.Message,
                Data = null!
            };
            return BadRequest(responseObj);
        }
    }

    /// <summary>
    /// Edit a collection of items.
    /// </summary>
    /// <param name="requestModels">Collection of items to edit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTTP 200 Ok, or 400 BadRequest.</returns>
    [HttpPut("items")]
    public async Task<ActionResult<ResponseObj<ICollection<TViewModel>>>> EditCollectionAsync([FromBody] ICollection<TViewModel> requestModels, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(requestModels);

            if (ModelState.IsValid)
            {
                var response = await requestHandler.EditCollectionAsync(requestModels, cancellationToken);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }

            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false,
                Message = $"Invalid request: {errors}",
                Data = null!
            });
        }
        catch (Exception ex)
        {
            var responseObj = new ResponseObj<ICollection<TViewModel>>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? "Failed to update data." : ex.Message,
                Data = null!
            };
            return BadRequest(responseObj);
        }
    }
}