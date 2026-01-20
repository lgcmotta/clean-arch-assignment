using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Models.Orders;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Orders;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Queries.Orders.Search;

[UsedImplicitly]
public sealed record SearchOrdersQuery : PagedQueryModel, IRequest<SearchOrdersResponse>, IQuery
{
    public required string CustomerId { get; init; }
}

public record SearchOrdersResponse(IEnumerable<OrderReadModel> Orders, PagedResponseModel Pagination);

public sealed class SearchOrdersQueryHandler(IOrderReadRepository repository) : IRequestHandler<SearchOrdersQuery, SearchOrdersResponse>
{
    public async Task<SearchOrdersResponse> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        var total = await repository.CountByCustomerAsync(request.CustomerId, cancellationToken);

        (IEnumerable<OrderReadModel> models, PagedResponseModel pagination) = await repository.GetOrdersByCustomerAsync(request.CustomerId, total, request, cancellationToken);

        return new SearchOrdersResponse(models, pagination);
    }
}