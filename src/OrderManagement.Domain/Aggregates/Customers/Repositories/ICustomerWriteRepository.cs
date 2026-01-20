using OrderManagement.Domain.Aggregates.Orders;

namespace OrderManagement.Domain.Aggregates.Customers.Repositories;

public interface ICustomerWriteRepository
{
    ValueTask<Customer?> FindCustomerByIdAsync(long id, CancellationToken cancellationToken = default);

    ValueTask<(Customer?, Order?)> FindCustomerWithOrderAsync(long customerId, long orderId, CancellationToken cancellationToken = default);

    ValueTask<bool> CheckIfEmailIsAlreadyTakenAsync(string email, CancellationToken cancellationToken = default);
    ValueTask AddNewCustomerAsync(Customer customer, CancellationToken cancellationToken = default);

    ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
}