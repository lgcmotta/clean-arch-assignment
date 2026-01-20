using HashidsNet;
using MediatR;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Domain.Aggregates.Products.Repositories;

namespace OrderManagement.Application.Commands.Orders.Update;

internal sealed class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, UpdateOrderResponse>
{
    private readonly IOrderWriteRepository _orders;
    private readonly IProductWriteRepository _products;
    private readonly IHashids _hashids;

    public UpdateOrderCommandHandler(IOrderWriteRepository orders, IProductWriteRepository products, IHashids hashids)
    {
        _orders = orders;
        _products = products;
        _hashids = hashids;
    }

    public async Task<UpdateOrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var customerId = _hashids.DecodeSingleLong(request.CustomerId);

        var orderId = _hashids.DecodeSingleLong(request.OrderId);

        var order = await _orders.FindCustomerOrderAsync(customerId, orderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(request.OrderId);
        }

        order.ChangeStatus(request.Status);

        if (request.Items is not null)
        {
            var productIds = request.Items.Select(item => item.ProductId)
                .Distinct()
                .Select(productId => _hashids.DecodeSingleLong(productId))
                .ToArray();

            var products = await _products.FindProductsAsync(productIds, cancellationToken);

            var productsMap = products.ToDictionary(product => product.Id);

            if (productsMap is { Count: <= 0 })
            {
                throw new ProductsNotFoundException(request.Items.Select(item => item.ProductId).ToArray());
            }

            foreach (var item in request.Items)
            {
                var productId = _hashids.DecodeSingleLong(item.ProductId);

                if (!productsMap.TryGetValue(productId, out var product))
                {
                    throw new ProductNotFoundException(item.ProductId);
                }

                order.SetItemQuantity(product.Id, product.Name, item.Quantity, product.Price);
            }
        }

        await _orders.SaveChangesAsync(cancellationToken);

        order.RaiseOrderUpdatedEvent();

        return new UpdateOrderResponse(request.OrderId, request.CustomerId, order.Status.Value);
    }
}