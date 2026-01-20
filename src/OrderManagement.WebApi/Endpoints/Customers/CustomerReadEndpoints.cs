using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Queries.Customers.Search;
using OrderManagement.WebApi.Requests;
using OrderManagement.WebApi.Responses;
using System.Net.Mime;

namespace OrderManagement.WebApi.Endpoints.Customers;

internal static class CustomerReadEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        internal IEndpointRouteBuilder MapGetCustomers(ApiVersion version)
        {
            builder.MapGet("customers", SearchCustomersAsync)
                .WithName($"get-v{version:V}-search-customers")
                .WithDisplayName("Search Customers")
                .WithTags("customers")
                .MapToApiVersion(version)
                .Produces<PagedApiResponse<IEnumerable<CustomerReadModel>>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        private static async Task<IResult> SearchCustomersAsync(
            [FromServices] IMediator mediator,
            [AsParameters] PaginationRequest pagination,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchCustomersQuery
            {
                Page = pagination.Page,
                Size = pagination.Size,
                Sort = pagination.Sort,
                SortBy = pagination.SortBy
            };

            var response = await mediator.Send(query, cancellationToken);

            return Results.Ok(new PagedApiResponse<IEnumerable<CustomerReadModel>>(response.Customers, response.Pagination));
        }
    }
}
