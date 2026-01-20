using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands.Products.CreateProduct;
using OrderManagement.WebApi.Responses;
using System.Net.Mime;

namespace OrderManagement.WebApi.Endpoints.Products;

internal static class ProductWriteEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        internal IEndpointRouteBuilder MapPostProducts(ApiVersion version)
        {
            builder.MapPost("products", CreateProductAsync)
                .WithName($"post-v{version:V}-create-product")
                .WithDisplayName("Create New Product")
                .WithTags("products")
                .MapToApiVersion(version)
                .Produces<ApiResponse<CreateProductResponse>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status409Conflict, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        private static async Task<IResult> CreateProductAsync(
            [FromServices] IMediator mediator,
            [FromBody] CreateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var response = await mediator.Send(command, cancellationToken);

            return Results.Created($"/products/{response.Id}", new ApiResponse<CreateProductResponse>(response));
        }
    }
}
