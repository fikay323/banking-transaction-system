using Microsoft.AspNetCore.Mvc;
using practice.Application.Interfaces;

namespace practice.API.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;

        public IdempotencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply to POST endpoints (since this governs Transactions)
            if (context.Request.Method != HttpMethods.Post)
            {
                await _next(context);
                return;
            }

            var endpoint = context.Request.Path.Value;
            if (!endpoint!.Contains("/transfer", StringComparison.OrdinalIgnoreCase) && 
                !endpoint.Contains("/airtime", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Must ensure idempotency key exists strictly
            if (!context.Request.Headers.TryGetValue("x-idempotency-key", out var extractedKey))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Missing Idempotency Key",
                    Detail = "The x-idempotency-key header is absolutely mandatory for financial operations to prevent duplicate executions."
                });
                return;
            }

            if (!Guid.TryParse(extractedKey, out var idempotencyKey))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Malformed Idempotency Key",
                    Detail = "The x-idempotency-key header must be a standard GUID."
                });
                return;
            }

            // Fetch via RequestServices explicitly to respect scoped lifetimes
            var idempotencyRepository = context.RequestServices.GetRequiredService<IIdempotencyRepository>();
            var isUniquePayload = await idempotencyRepository.EnsureIdempotencyAsync(idempotencyKey, endpoint);

            if (!isUniquePayload)
            {
                context.Response.StatusCode = 409;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Duplicate Process Execution",
                    Detail = "A request utilizing this specific idempotency key has already been successfully evaluated. The system blocked the duplication attempt natively."
                });
                return;
            }

            await _next(context);
        }
    }
}
