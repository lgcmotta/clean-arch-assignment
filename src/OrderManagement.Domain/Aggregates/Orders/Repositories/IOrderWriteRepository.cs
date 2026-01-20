namespace OrderManagement.Domain.Aggregates.Orders.Repositories;

public interface IOrderWriteRepository
{
    ValueTask AddOrderAsync(Order order, CancellationToken cancellationToken = default);

    ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);

    ValueTask<Order?> FindCustomerOrderAsync(long customerId, long orderId, CancellationToken cancellationToken = default);

    ValueTask<Order?> FindUntrackedCustomerOrderAsync(long customerId, long orderId, CancellationToken cancellationToken = default);
}