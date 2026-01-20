using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands.Products.CreateProduct;
using OrderManagement.Application.Models.Products;
using OrderManagement.Application.Queries.Products.Search;
using OrderManagement.WebApi.Responses;
using System.Net.Mime;

namespace OrderManagement.WebApi.Endpoints;

internal static class ProductEndpoints
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
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        internal IEndpointRouteBuilder MapGetProducts(ApiVersion version)
        {
            builder.MapGet("products", SearchProductsAsync)
                .WithName($"get-v{version:V}-search-produces")
                .WithDisplayName("Search Products")
                .WithTags("products")
                .MapToApiVersion(version)
                .Produces<PagedApiResponse<IEnumerable<ProductReadModel>>>(contentType: MediaTypeNames.Application.Json)
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

        private static async Task<IResult> SearchProductsAsync(
            [FromServices] IMediator mediator,
            [AsParameters] SearchProductsQuery query,
            CancellationToken cancellationToken = default)
        {
            var response = await mediator.Send(query, cancellationToken);

            return Results.Ok(new PagedApiResponse<IEnumerable<ProductReadModel>>(response.Products, response.Pagination));
        }
    }
}