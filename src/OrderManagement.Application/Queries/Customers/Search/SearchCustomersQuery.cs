using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Customers;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Queries.Customers.Search;

[UsedImplicitly]
public record SearchCustomersQuery : PagedQueryModel, IRequest<SearchCustomersResponse>, IQuery;

public record SearchCustomersResponse(IEnumerable<CustomerReadModel> Customers, PagedResponseModel Pagination);

public sealed class SearchCustomersQueryHandler(ICustomerReadRepository repository) : IRequestHandler<SearchCustomersQuery, SearchCustomersResponse>
{
    public async Task<SearchCustomersResponse> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var total = await repository.CountTotalCustomersAsync(cancellationToken);

        (IEnumerable<CustomerReadModel> models, PagedResponseModel pagination) =
            await repository.PaginateCustomersAsync(total, request, cancellationToken);

        return new SearchCustomersResponse(models, pagination);
    }
}
