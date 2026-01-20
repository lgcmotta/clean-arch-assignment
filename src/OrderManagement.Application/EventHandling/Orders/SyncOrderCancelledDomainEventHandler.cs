using HashidsNet;
using OrderManagement.Application.Repositories.Orders;
using OrderManagement.Domain.Aggregates.Orders.Events;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.EventHandling.Orders;

public sealed class SyncOrderCancelledDomainEventHandler : IDomainEventHandler<OrderCancelledDomainEvent>
{
    private readonly IOrderReadRepository _reader;
    private readonly IHashids _hashids;

    public SyncOrderCancelledDomainEventHandler(IOrderReadRepository reader, IHashids hashids)
    {
        _reader = reader;
        _hashids = hashids;
    }

    public async Task Handle(OrderCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        var customerId = _hashids.EncodeLong(notification.CustomerId);

        var orderId = _hashids.EncodeLong(notification.Id);

        await _reader.SyncOrderCancelledAsync(customerId, orderId, cancellationToken);
    }
}