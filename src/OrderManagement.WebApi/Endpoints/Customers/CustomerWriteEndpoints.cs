using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands.Customers.Create;
using OrderManagement.WebApi.Responses;
using System.Net.Mime;

namespace OrderManagement.WebApi.Endpoints.Customers;

internal static class CustomerWriteEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        internal IEndpointRouteBuilder MapPostCustomer(ApiVersion version)
        {
            builder.MapPost("customers", CreateCustomerAsync)
                .WithName($"post-v{version:V}-create-customer")
                .WithDisplayName("Create New Customer")
                .WithTags("customers")
                .MapToApiVersion(version)
                .Produces<ApiResponse<CreateCustomerResponse>>(contentType: MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
                .Produces<ProblemDetails>(statusCode: StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.ProblemJson);

            return builder;
        }

        private static async Task<IResult> CreateCustomerAsync(
            [FromServices] IMediator mediator,
            [FromBody] CreateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            var response = await mediator.Send(command, cancellationToken);

            return Results.Created($"/customers/{response.Id}", new ApiResponse<CreateCustomerResponse>(response));
        }
    }
}