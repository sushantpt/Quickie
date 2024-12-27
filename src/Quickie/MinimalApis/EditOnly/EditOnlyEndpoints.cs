namespace Quickie.MinimalApis.EditOnly;

/// <summary>
/// Provides extension methods for adding edit-only endpoints
/// </summary>
public static class EditOnlyEndpoints
{
    /// <summary>
    /// Adds edit-only endpoints for single and collection-based item updates
    /// </summary>
    /// <param name="app">The IEndpointRouteBuilder instance to add endpoints to</param>
    /// <param name="baseRoute">The base route for all CRUD endpoints <c>(e.g., "/api/resources")</c></param>
    /// <typeparam name="TViewModel">The type of the view model/DTO used for data transfer of type CrudDto</typeparam>
    /// <typeparam name="TRequestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</typeparam>
    /// <typeparam name="TIdType">The type of the primary key/identifier of type IComparable</typeparam>
    /// <remarks>
    /// Adds the following endpoints:
    /// - PUT {baseRoute}/{id} - Edits a single resource
    /// - PUT {baseRoute}/items - Edits multiple resources
    /// </remarks>
    public static void AddEditOnlyEndpoints<TViewModel, TRequestHandler, TIdType>(this IEndpointRouteBuilder app, string baseRoute) 
        where TViewModel : EditOnlyDto where TRequestHandler : IEditOnlyRequestHandler<TViewModel, TIdType> where TIdType : IComparable
    {
        var options = GlobalQuickieConfigData.Options;

        // Edit single item
        app.MapPut($"{baseRoute}/{{id}}", async Task<IResult> (TRequestHandler requestHandler, TIdType id, [FromBody] TViewModel requestModel, CancellationToken cancellationToken) => 
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                ArgumentNullException.ThrowIfNull(requestModel);
                
                if (typeof(TIdType) == typeof(int) && id is < 1)
                {
                    return Results.BadRequest();
                }

                var response = await requestHandler.EditAsync(id, requestModel, cancellationToken);
                return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? "Failed to update data." : ex.Message,
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });

        // Edit collection
        app.MapPut($"{baseRoute}/items", async Task<IResult> (TRequestHandler requestHandler, [FromBody] ICollection<TViewModel> requestModels, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(requestModels);

                var response = await requestHandler.EditCollectionAsync(requestModels, cancellationToken);
                return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<ICollection<TViewModel>>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? "Failed to update data." : ex.Message,
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });
    }
}