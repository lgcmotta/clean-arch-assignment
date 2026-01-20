using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands.Orders.Cancel;
using OrderManagement.Application.Commands.Orders.Create;
using OrderManagement.Application.Commands.Orders.Update;
using OrderManagement.WebApi.Responses;
using System.Net.Mime;

namespace OrderManagement.WebApi.Endpoints.Orders;

internal static class OrderWriteEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        internal IEndpointRouteBuilder MapPostOrders(ApiVersion version)
        {
            builder.MapPost("customers/{customerId}/orders", CreateOrderAsync)
                .WithName($"post-v{version:V}-create-order")
                .WithDisplayName("Create New Order")
                .WithTags("orders")
                .MapToApiVersion(version)
                .Produces<ApiResponse<CreateOrderResponse>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        internal IEndpointRouteBuilder MapPatchOrders(ApiVersion version)
        {
            builder.MapPatch("customers/{customerId}/orders/{orderId}", UpdateOrderAsync)
                .WithName($"patch-v{version:V}-update-order")
                .WithDisplayName("Update Order")
                .WithTags("orders")
                .MapToApiVersion(version)
                .Produces<ApiResponse<UpdateOrderResponse>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        internal IEndpointRouteBuilder MapDeleteOrders(ApiVersion version)
        {
            builder.MapDelete("customers/{customerId}/orders/{orderId}", CancelOrderAsync)
                .WithName($"delete-v{version:V}-cancel-order")
                .WithDisplayName("Cancel Order")
                .WithTags("orders")
                .MapToApiVersion(version)
                .Produces<ApiResponse<CancelOrderResponse>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        private static async Task<IResult> CreateOrderAsync(
            [FromServices] IMediator mediator,
            [FromRoute] string customerId,
            [FromBody] CreateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            var response = await mediator.Send(command, cancellationToken);

            return Results.Created($"/customers/{response.CustomerId}/orders/{response.Id}", new ApiResponse<CreateOrderResponse>(response));
        }

        private static async Task<IResult> UpdateOrderAsync(
            [FromServices] IMediator mediator,
            [FromRoute] string customerId,
            [FromRoute] string orderId,
            [FromBody] UpdateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(new ApiResponse<UpdateOrderResponse>(response));
        }

        private static async Task<IResult> CancelOrderAsync(
            [FromServices] IMediator mediator,
            [FromRoute] string customerId,
            [FromRoute] string orderId,
            CancellationToken cancellationToken = default)
        {
            var command = new CancelOrderCommand(customerId, orderId);

            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(new ApiResponse<CancelOrderResponse>(response));
        }
    }
}