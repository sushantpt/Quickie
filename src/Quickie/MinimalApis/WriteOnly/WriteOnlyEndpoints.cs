namespace Quickie.MinimalApis.WriteOnly;

/// <summary>
/// Provides extension methods for adding write-only (POST) endpoints
/// </summary>
public static class WriteOnlyEndpoints
{
    /// <summary>
    /// Adds write-only endpoints for creating single or bulk resources
    /// </summary>
    /// <param name="app">The IEndpointRouteBuilder instance to add endpoints to</param>
    /// <param name="baseRoute">The base route for all CRUD endpoints <c>(e.g., "/api/resources")</c></param>
    /// <typeparam name="TViewModel">The type of the view model/DTO used for data transfer of type CrudDto</typeparam>
    /// <typeparam name="TRequestHandler">The type of the request handler that implements business logic of type ICrudRequestHandler</typeparam>
    public static void AddWriteOnlyEndpoints<TViewModel, TRequestHandler>(this IEndpointRouteBuilder app, string baseRoute)
        where TViewModel : WriteOnlyDto where TRequestHandler : IWriteOnlyRequestHandler<TViewModel>
    {
        var options = GlobalQuickieConfigData.Options;

        // single
        app.MapPost(baseRoute, async Task<IResult> (TRequestHandler requestHandler, [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] TViewModel request, CancellationToken cancellationToken) =>
        {
            try
            {
                if (!ValidateModelState(request, out var validationErrors))
                {
                    return Results.BadRequest(new ResponseObj<TViewModel>
                    {
                        IsSuccess = false,
                        Message = $"Invalid request: {validationErrors}",
                        Data = null!
                    });
                }

                var result = await requestHandler.CreateItemAsync(request, cancellationToken);
                return result.IsSuccess ? Results.Created($"{baseRoute}", result) : Results.BadRequest(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ResponseObj<TViewModel>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? "Failed to create request." : ex.Message,
                    Data = null!
                });
            }
        });

        // multiple items
        app.MapPost($"{baseRoute}/items", async Task<IResult> (TRequestHandler requestHandler, [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey, [FromBody] ICollection<TViewModel> requests, CancellationToken cancellationToken) =>
        {
            try
            {
                if (!ValidateModelState(requests, out var validationErrors))
                {
                    return Results.BadRequest(new ResponseObj<ICollection<TViewModel>>
                    {
                        IsSuccess = false,
                        Message = $"Invalid request: {validationErrors}",
                        Data = null!
                    });
                }

                var result = await requestHandler.CreateItemsAsync(requests, cancellationToken);
                return result.IsSuccess ? Results.Created($"{baseRoute}/items", result) : Results.BadRequest(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ResponseObj<ICollection<TViewModel>>
                {
                    IsSuccess = false,
                    Message = options is { ShowCustomErrorMessage: true } ? "An error occurred while creating the items." : ex.Message,
                    Data = null!
                });
            }
        });
    }

    /// <summary>
    /// Model state validator
    /// </summary>
    /// <param name="model"></param>
    /// <param name="validationErrors"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static bool ValidateModelState<T>(T model, out string validationErrors)
    {
        var errors = new List<string>();
        if (model == null)
        {
            errors.Add("Request cannot be null.");
        }
        validationErrors = string.Join(", ", errors);
        return !errors.Any();
    }
}