namespace Quickie.MinimalApis.ReadOnly;

/// <summary>
/// Provides extension methods for adding read-only (GET) endpoints
/// </summary>
public static class ReadOnlyEndpoints
{
    /// <summary>
    /// Adds read-only endpoints for retrieving resources by ID.
    /// </summary>
    /// <param name="app">The IEndpointRouteBuilder instance to add endpoints to</param>
    /// <param name="baseRoute">The base route for all CRUD endpoints <c>(e.g., "/api/resources")</c></param>
    /// <typeparam name="TViewModel">The type of the view model/DTO used for data transfer of type CrudDto</typeparam>
    /// <typeparam name="TRequestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</typeparam>
    /// <typeparam name="TIdType">The type of the primary key/identifier of type IComparable</typeparam>
    /// <remarks>
    /// Adds the following endpoint:
    /// - GET {baseRoute}/{id} - Retrieves a specific resource by ID
    /// </remarks>
    public static void AddReadOnlyEndpoints<TViewModel, TRequestHandler, TIdType>(this IEndpointRouteBuilder app, string baseRoute) 
        where TViewModel : ReadOnlyDto where TRequestHandler : IReadOnlyRequestHandler<TViewModel, TIdType> where TIdType : IComparable
    {
        var options = GlobalQuickieConfigData.Options;

        app.MapGet($"{baseRoute}/{{id}}", async Task<IResult> (TRequestHandler requestHandler, TIdType id, CancellationToken cancellationToken) =>
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(nameof(id));
                if (typeof(TIdType) == typeof(int) && id is < 1)
                {
                    return Results.BadRequest();
                }

                var data = await requestHandler.GetByIdAsync(id, cancellationToken);
                return data.IsSuccess 
                    ? Results.Ok(data) 
                    : Results.NotFound(data);
            }
            catch (Exception ex)
            {
                var responseObj = new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = options.ShowCustomErrorMessage ? "Failed to get data." : ex.Message,
                    Data = null!
                };
                return Results.BadRequest(responseObj);
            }
        });
    }
}