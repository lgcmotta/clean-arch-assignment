using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Models.Shared;

namespace OrderManagement.Application.Repositories.Customers;

public interface ICustomerReadRepository
{
    ValueTask SyncReadModelAsync(CustomerReadModel model, CancellationToken cancellationToken = default);

    ValueTask<(IEnumerable<CustomerReadModel>, PagedResponseModel)> PaginateCustomersAsync(long total, PagedQueryModel query, CancellationToken cancellationToken = default);

    ValueTask<long> CountTotalCustomersAsync(CancellationToken cancellationToken = default);
}