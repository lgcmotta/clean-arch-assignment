using Microsoft.AspNetCore.Diagnostics;
using OrderManagement.WebApi.Extensions;
using FluentValidation;
using OrderManagement.Domain.Aggregates.Customers.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Exceptions;
using System.Net.Mime;
using System.Text.Json;

namespace OrderManagement.WebApi.Diagnostics;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var instance = httpContext.Request.Path.Value ?? string.Empty;

        var problemDetails = exception switch
        {
            ValidationException e => e.ToProblemDetails(instance),
            ArgumentNullException e => e.ToProblemDetails(instance),
            ArgumentOutOfRangeException e => e.ToProblemDetails(instance),
            ArgumentException e => e.ToProblemDetails(instance),
            DuplicatedProductNameException e => e.ToProblemDetails(instance),
            ProductNotFoundForSyncException e => e.ToProblemDetails(instance),
            CustomerEmailAlreadySignedUpException e => e.ToProblemDetails(instance),
            CustomerNotFoundException e => e.ToProblemDetails(instance),
            OrderNotFoundException e => e.ToProblemDetails(instance),
            OrderStatusTransitionException e => e.ToProblemDetails(instance),
            ProductNotFoundException e => e.ToProblemDetails(instance),
            ProductsNotFoundException e => e.ToProblemDetails(instance),
            _ => exception.ToProblemDetails(instance)
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType = MediaTypeNames.Application.ProblemJson;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, JsonSerializerOptions.Web, cancellationToken);

        return true;
    }
}