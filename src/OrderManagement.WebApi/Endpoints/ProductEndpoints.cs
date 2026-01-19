using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands.Products.CreateProduct;
using OrderManagement.Application.Queries.SearchProducts;
using OrderManagement.Application.Shared;
using OrderManagement.WebApi.Requests;
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
                .Produces<PagedApiResponse<IEnumerable<ProductResponse>>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        private static async Task<IResult> CreateProductAsync(
            [FromServices] IMediator mediator,
            [FromBody] CreateProductCommand body,
            CancellationToken cancellationToken = default)
        {
            var response = await mediator.Send(body, cancellationToken);

            return Results.Created($"/products/{response.Id}", new ApiResponse<CreateProductResponse>(response));
        }

        private static async Task<IResult> SearchProductsAsync(
            [FromServices] IMediator mediator,
            [AsParameters] PaginationRequest pagination,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchProductsQuery(pagination.Page, pagination.Size, pagination.Sort, pagination.SortBy);

            var response = await mediator.Send(query, cancellationToken);

            return Results.Ok(new PagedApiResponse<IEnumerable<ProductResponse>>(response.Products,
                new PagedResponse(response.Pagination.Page, response.Pagination.Previous, response.Pagination.Next, response.Pagination.Total)));
        }
    }
}