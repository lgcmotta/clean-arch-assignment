using Microsoft.AspNetCore.Diagnostics;

namespace OrderManagement.WebApi.Middlewares;

internal sealed class GlobalExceptionHandlerMiddleware(IExceptionHandler handler) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception exception)
        {
            await handler.TryHandleAsync(context, exception, context.RequestAborted);
        }
    }
}