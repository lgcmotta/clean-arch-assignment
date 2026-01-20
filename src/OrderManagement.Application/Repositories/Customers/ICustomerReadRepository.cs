using OrderManagement.Application.Models.Customer;

namespace OrderManagement.Application.Repositories.Customers;

public interface ICustomerReadRepository
{
    ValueTask SyncReadModelAsync(CustomerReadModel model, CancellationToken cancellationToken = default);
}