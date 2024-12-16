using System.Text.Json;

namespace Quickie.Configuration.Middleware;

/// <summary>
/// Idempotency implementer middleware 
/// </summary>
/// <param name="idempotencyProvider">Idempotency provider</param>
/// <param name="next"></param>
public class IdempotencyMiddleware(IIdempotencyProvider idempotencyProvider, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        /*
         * Idempotency for only POST request.
         * https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST
         * https://developer.mozilla.org/en-US/docs/Glossary/Idempotent
         */
        if (context.Request.Method == HttpMethod.Post.Method)
        {
            if (!context.Request.Headers.TryGetValue("X-Idempotency-Key", out var idempotencyKey) ||
                string.IsNullOrWhiteSpace(idempotencyKey))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errorResponse = JsonSerializer.Serialize(new 
                {
                    message = "X-Idempotency-Key header is missing or invalid.",
                    status = StatusCodes.Status400BadRequest
                });
                await context.Response.WriteAsync(errorResponse);
                return;
            }

            if (await idempotencyProvider.ExistsAsync(idempotencyKey!))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                var errorResponse = JsonSerializer.Serialize(new 
                {
                    message = "Duplicate request. This operation has already been processed.",
                    status = StatusCodes.Status409Conflict
                });
                await context.Response.WriteAsync(errorResponse);
                return;
            }

            await idempotencyProvider.MarkAsync(idempotencyKey!);
        }
        await next(context);
    }
}