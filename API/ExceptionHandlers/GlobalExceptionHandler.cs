using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace practice.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            switch (exception)
            {
                // Core Banking & Missing Fund Errors Handled Normally
                case InvalidOperationException invalidOpEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Operation Validation Terminated";
                    problemDetails.Detail = invalidOpEx.Message; // Propagates "Insufficient Funds" natively
                    break;

                // Constructor rules (e.g. invalid arguments)
                case ArgumentException argEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Payload Property Violated";
                    problemDetails.Detail = argEx.Message;
                    break;

                // Wildcard safety net
                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Critical Failure Envelope";
                    problemDetails.Detail = "An unexpected server condition halted the routine processing constraints.";
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            
            return true; // We successfully handled formatting
        }
    }
}
