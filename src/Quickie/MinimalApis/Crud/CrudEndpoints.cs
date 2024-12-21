namespace Quickie.MinimalApis.Crud;

/// <summary>
/// Provides extension methods for CRUD endpoints.
/// </summary>
public static class CrudEndpoints
{
    /// <summary>
    /// Providing CRUD endpoints
    /// </summary>
    /// <param name="app">The IEndpointRouteBuilder instance to add endpoints to</param>
    /// <param name="baseRoute">The base route for all CRUD endpoints <c>(e.g., "/api/resources")</c></param>
    /// <param name="requestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</param>
    /// <typeparam name="TViewModel">The type of the view model/DTO used for data transfer of type CrudDto</typeparam>
    /// <typeparam name="TRequestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</typeparam>
    /// <typeparam name="TIdType">The type of the primary key/identifier of type IComparable</typeparam>
    /// <remarks>
    /// Adds the following endpoints:
    /// - POST {baseRoute} - Creates a new resource
    /// - GET {baseRoute}/{id} - Retrieves a specific resource by ID
    /// - PUT {baseRoute}/{id} - Updates a specific resource
    /// - DELETE {baseRoute}/{id} - Deletes a specific resource
    /// </remarks>
    /// <example>
    /// <code>
    /// app.AddEndpoints&lt;TodoDto, ITodoRequestHandler, int&gt;(
    ///     "/api/users",
    ///     new UserRequestHandler()
    /// );
    /// </code>
    /// </example>
    public static void AddEndpoints<TViewModel, TRequestHandler, TIdType>(this IEndpointRouteBuilder app, string baseRoute, TRequestHandler requestHandler) 
        where TViewModel : CrudDto where TRequestHandler : ICrudRequestHandler<TViewModel, TIdType> where TIdType : IComparable 
    {
        var options = GlobalQuickieConfigData.Options;
        
        // Create
        app.MapPost(baseRoute, async Task<IResult> ([FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] TViewModel request, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                
                var response = await requestHandler.CreateAsync(request, cancellationToken);
                return response is { IsSuccess: true } ? Results.Created($"{baseRoute}/", response) : Results.BadRequest(response);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to create request.",
                    Data = request
                };
                return Results.BadRequest(responseObj);
            }
        });

        // Get by ID
        app.MapGet($"{baseRoute}/{{id}}", async Task<IResult> (TIdType id, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                if (typeof(TIdType) == typeof(int) && id is < 1)
                {
                    return Results.BadRequest();
                }

                var data = await requestHandler.GetByIdAsync(id, cancellationToken);
                return data is { IsSuccess: true } ? Results.Ok(data) : Results.NotFound(data);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to get data.",
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });

        // Update
        app.MapPut($"{baseRoute}/{{id}}", async Task<IResult> (TIdType id, [FromBody] TViewModel request, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                if (typeof(TIdType) == typeof(int) && id is < 1)
                {
                    return Results.BadRequest();
                }

                var update = await requestHandler.UpdateAsync(id, request, cancellationToken);
                return update is { IsSuccess: true } ? Results.Ok(update) : Results.BadRequest(update);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? ex.Message : "Failed to update request.",
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });

        // Delete
        app.MapDelete($"{baseRoute}/{{id}}", async Task<IResult> (TIdType id, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                if (typeof(TIdType) == typeof(int) && id is < 1)
                {
                    return Results.BadRequest();
                }

                var delete = await requestHandler.DeleteAsync(id, cancellationToken);
                return delete is { IsSuccess: true } ? Results.NoContent() : Results.BadRequest(delete);
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