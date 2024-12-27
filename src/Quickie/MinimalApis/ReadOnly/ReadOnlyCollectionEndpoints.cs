namespace Quickie.MinimalApis.ReadOnly;

/// <summary>
/// Provides extension methods for adding read-only (GET) collection endpoints
/// </summary>
public static class ReadOnlyCollectionEndpoints
{
    /// <summary>
    /// Adds read-only endpoints for retrieving a collection of resources with filtering, pagination, etc.
    /// </summary>
    /// <param name="app">The IEndpointRouteBuilder instance to add endpoints to</param>
    /// <param name="baseRoute">The base route for all CRUD endpoints <c>(e.g., "/api/resources")</c></param>
    /// <typeparam name="TViewModel">The type of the view model/DTO used for data transfer of type CrudDto</typeparam>
    /// <typeparam name="TRequestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</typeparam>
    /// <typeparam name="TIdType">The type of the primary key/identifier of type IComparable</typeparam>
    /// <typeparam name="TRequestForDataModel">Data request model of type RequestForDataObj.</typeparam>
    /// <remarks>
    /// Adds the following endpoint:
    /// - GET {baseRoute}/items - Retrieves collection of specific resource based on request
    /// </remarks>
    public static void AddReadOnlyCollectionEndpoints<TViewModel, TRequestHandler, TRequestForDataModel, TIdType>(this IEndpointRouteBuilder app, string baseRoute) 
        where TViewModel : ReadOnlyDto
        where TRequestHandler : IReadOnlyCollectionRequestHandler<TViewModel, TIdType>
        where TRequestForDataModel : RequestForDataObj
        where TIdType : IComparable
    {
        var options = GlobalQuickieConfigData.Options;

        app.MapGet($"{baseRoute}/items", async Task<IResult> (TRequestHandler requestHandler, [AsParameters] TRequestForDataModel request, CancellationToken cancellationToken) =>
        {
            try
            {
                if (!ValidateModelState(request, out var validationErrors))
                {
                    return Results.BadRequest(new
                    {
                        Message = $"Invalid request: {validationErrors}"
                    });
                }

                var data = await requestHandler.GetAsync(request, cancellationToken);
                if (data.Items != null && data.Items.Any() && data.Total > 0)
                {
                    return Results.Ok(data);
                }

                return Results.NotFound(new { Message = "No items found matching the query parameters." });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    Message = options is { ShowCustomErrorMessage: true } ? "Failed to get data." : ex.Message
                });
            }
        });
    }

    /// <summary>
    /// Model state validator
    /// </summary>
    /// <param name="model"></param>
    /// <param name="validationErrors"></param>
    /// <returns></returns>
    private static bool ValidateModelState(object model, out string validationErrors)
    {
        var errors = new List<string>();

        if (model is null)
        {
            errors.Add("Request cannot be null.");
        }

        validationErrors = string.Join(", ", errors);
        return !errors.Any();
    }
}