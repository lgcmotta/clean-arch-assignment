using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Models.Orders;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Queries.Orders.Search;

[UsedImplicitly]
public sealed record SearchOrdersQuery : PagedQueryModel, IRequest<SearchOrdersResponse>, IQuery
{
    public required string CustomerId { get; init; }
}

public record SearchOrdersResponse(IEnumerable<OrderReadModel> Orders, PagedResponseModel Pagination);