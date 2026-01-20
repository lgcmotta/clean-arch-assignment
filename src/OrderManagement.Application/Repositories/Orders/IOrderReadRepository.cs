using OrderManagement.Application.Models.Orders;
using OrderManagement.Application.Models.Shared;

namespace OrderManagement.Application.Repositories.Orders;

public interface IOrderReadRepository
{
    ValueTask<(IEnumerable<OrderReadModel>, PagedResponseModel)> GetOrdersByCustomerAsync(string customerId,
        long total,
        PagedQueryModel query,
        CancellationToken cancellationToken = default);

    ValueTask<long> CountByCustomerAsync(string customerId, CancellationToken cancellationToken = default);

    ValueTask SyncOrderCancelledAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    ValueTask SyncOrderCreatedAsync(OrderReadModel model, CancellationToken cancellationToken = default);

    ValueTask SyncOrderUpdatedAsync(OrderReadModel model, CancellationToken cancellationToken = default);
}