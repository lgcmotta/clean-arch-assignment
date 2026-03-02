using HashidsNet;
using MediatR;
using OrderManagement.Domain.Aggregates.Orders.Entities;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Repositories;

namespace OrderManagement.Application.Commands.Orders.RemoveItem;

internal sealed class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, OrderItemRemovedResponse>
{
    private readonly IOrderWriteRepository _orders;
    private readonly IHashids _hashids;

    public RemoveOrderItemCommandHandler(IOrderWriteRepository orders, IHashids hashids)
    {
        _orders = orders;
        _hashids = hashids;
    }

    public async Task<OrderItemRemovedResponse> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        var customerId = _hashids.DecodeSingleLong(request.CustomerId);

        var orderId = _hashids.DecodeSingleLong(request.OrderId);

        var productId = _hashids.DecodeSingleLong(request.ProductId);

        var order = await _orders.FindCustomerOrderAsync(customerId, orderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(request.OrderId);
        }

        order.RemoveItem(productId);

        await _orders.SaveChangesAsync(cancellationToken);

        order.RaiseOrderUpdatedEvent();

        RemoveOrderItemResponse[] items = [..order.Items.Select(item => item.ToResponse(_hashids))];

        return new OrderItemRemovedResponse(request.OrderId, request.CustomerId, order.Status.Value, items);
    }
}

file static class PatchOrderItemExtensions
{
    extension(OrderItem item)
    {
        internal RemoveOrderItemResponse ToResponse(IHashids hashids)
        {
            return new RemoveOrderItemResponse
            {
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                UnitPrice = item.UnitPrice,
                ProductId = hashids.EncodeLong(item.ProductId),
                ProductName = item.ProductName,
            };
        }
    }
}