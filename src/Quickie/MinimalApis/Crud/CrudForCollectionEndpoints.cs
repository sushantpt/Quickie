namespace Quickie.MinimalApis.Crud;

/// <summary>
/// Provides extension methods for CRUD of collection endpoints.
/// </summary>
public static class CrudForCollectionEndpoints
{
    /// <summary>
    /// Provides CRUD for collection endpoints
    /// </summary>
    /// <param name="app">The IEndpointRouteBuilder instance to add endpoints to</param>
    /// <param name="baseRoute">The base route for all CRUD endpoints <c>(e.g., "/api/resources")</c></param>
    /// <param name="requestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</param>
    /// <typeparam name="TViewModel">The type of the view model/DTO used for data transfer of type CrudDto</typeparam>
    /// <typeparam name="TRequestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</typeparam>
    /// <typeparam name="TIdType">The type of the primary key/identifier of type IComparable</typeparam>
    /// <remarks>
    /// Adds the following endpoints:
    /// - POST {baseRoute}/items - Creates multiple resources
    /// - PUT {baseRoute}/items - Updates multiple resources
    /// - GET {baseRoute}/items - Gets paginated list of resources
    /// - GET {baseRoute}/items/count - Gets total count of resources
    /// - DELETE {baseRoute}/items - Deletes multiple resources
    /// </remarks>
    public static void AddCollectionEndpoints<TViewModel, TRequestHandler, TIdType>(this IEndpointRouteBuilder app, string baseRoute, TRequestHandler requestHandler) 
        where TViewModel : CrudDto where TRequestHandler : ICrudForCollectionRequestHandler<TViewModel, TIdType> where TIdType : IComparable
    {
        var options = GlobalQuickieConfigData.Options;

        // create collection
        app.MapPost($"{baseRoute}/items", async Task<IResult> ([FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] ICollection<TViewModel> request,  CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                
                var create = await requestHandler.CreateRangeAsync(request, cancellationToken);
                return create.IsSuccess ? Results.Created($"{baseRoute}/items", create) : Results.BadRequest(create);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<ICollection<TViewModel>>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to create request.",
                    Data = request
                };
                return Results.BadRequest(responseObj);
            }
        });

        // update collection
        app.MapPut($"{baseRoute}/items", async Task<IResult> ([FromBody] ICollection<TViewModel> request, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);

                var update = await requestHandler.UpdateRangeAsync(request, cancellationToken);
                return update.IsSuccess ? Results.Ok(update) : Results.BadRequest(update);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<ICollection<TViewModel>>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to update request.",
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });

        // get all (paginated)
        app.MapGet($"{baseRoute}/items", async Task<IResult> ([FromBody] RequestForDataObj request, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);

                var data = await requestHandler.GetAllAsync(request, cancellationToken);
                return data.IsSuccess ? Results.Ok(data) : Results.NotFound(data);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<PaginatedDataObj<TViewModel>>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to get data.",
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });

        // get count
        app.MapGet($"{baseRoute}/items/count", async Task<IResult> (CancellationToken cancellationToken) =>
        {
            try
            {
                var count = await requestHandler.CountAsync(cancellationToken);
                return count.IsSuccess ? Results.Ok(count) : Results.BadRequest(count);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<int>
                {
                    IsSuccess = false,
                    Message = options.ShowCustomErrorMessage ? ex.Message : "Failed to get count.",
                    Data = 0
                };
                return Results.BadRequest(responseObj);
            }
        });

        // delete collection
        app.MapDelete($"{baseRoute}/items", async Task<IResult> ([FromBody] ICollection<TIdType> ids, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(ids);

                var delete = await requestHandler.DeleteRangeAsync(ids, cancellationToken);
                return delete.IsSuccess ? Results.NoContent() : Results.BadRequest(delete);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to delete request."
                };
                return Results.BadRequest(responseObj);
            }
        });
    }
}