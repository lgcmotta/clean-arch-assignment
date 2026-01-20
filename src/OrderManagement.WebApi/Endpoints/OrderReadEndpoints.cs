using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Models;
using OrderManagement.Application.Models.Orders;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Queries.Orders.Search;
using OrderManagement.WebApi.Requests;
using OrderManagement.WebApi.Responses;
using System.Net.Mime;

namespace OrderManagement.WebApi.Endpoints;

internal static class OrderReadEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        internal IEndpointRouteBuilder MapGetOrders(ApiVersion version)
        {
            builder.MapGet("customers/{customerId}/orders", SearchOrdersAsync)
                .WithName($"get-v{version:V}-search-orders")
                .WithDisplayName("Search Orders")
                .WithTags("orders")
                .MapToApiVersion(version)
                .Produces<PagedApiResponse<IEnumerable<OrderReadModel>>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        private static async Task<IResult> SearchOrdersAsync(
            [FromServices] IMediator mediator,
            [FromRoute] string customerId,
            [AsParameters] PaginationRequest pagination,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchOrdersQuery
            {
                CustomerId = customerId,
                Page = pagination.Page,
                Size = pagination.Size,
                Sort = pagination.Sort,
                SortBy = pagination.SortBy
            };

            var response = await mediator.Send(query, cancellationToken);

            return Results.Ok(new PagedApiResponse<IEnumerable<OrderReadModel>>(response.Orders, response.Pagination));
        }
    }
}