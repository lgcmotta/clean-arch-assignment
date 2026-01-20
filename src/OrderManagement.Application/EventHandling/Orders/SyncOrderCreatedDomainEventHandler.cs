using HashidsNet;
using OrderManagement.Application.Models.Orders;
using OrderManagement.Application.Repositories.Orders;
using OrderManagement.Domain.Aggregates.Customers;
using OrderManagement.Domain.Aggregates.Customers.Exceptions;
using OrderManagement.Domain.Aggregates.Customers.Repositories;
using OrderManagement.Domain.Aggregates.Orders;
using OrderManagement.Domain.Aggregates.Orders.Events;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.EventHandling.Orders;

public sealed class SyncOrderCreatedDomainEventHandler : IDomainEventHandler<OrderCreatedDomainEvent>
{
    private readonly ICustomerWriteRepository _customers;
    private readonly IProductWriteRepository _products;
    private readonly IOrderReadRepository _orders;
    private readonly IHashids _hashids;

    public SyncOrderCreatedDomainEventHandler(ICustomerWriteRepository customers,
        IProductWriteRepository products,
        IOrderReadRepository orders,
        IHashids hashids)
    {
        _customers = customers;
        _orders = orders;
        _hashids = hashids;
        _products = products;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        (Customer? customer, Order? order) = await _customers.FindCustomerWithOrderAsync(notification.CustomerId, notification.OrderId, cancellationToken);

        if (customer is null)
        {
            throw new CustomerNotFoundException(_hashids.EncodeLong(notification.CustomerId));
        }

        if (order is null)
        {
            throw new OrderNotFoundException(_hashids.EncodeLong(notification.OrderId));
        }

        var productIds = order.Items.Select(item => item.ProductId).Distinct();

        var products = await _products.FindProductsAsync(productIds, cancellationToken);

        var productsMap = products.ToDictionary(product => product.Id);

        var orderId = _hashids.EncodeLong(order.Id);

        var customerId = _hashids.EncodeLong(customer.Id);

        var model = new OrderReadModel
        {
            Id = orderId,
            CreatedDate = order.CreatedDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.Value,
            Customer = new CustomerOrderReadModel
            {
                Id = customerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone
            },
            Items =
            [
                ..order.Items.Select(item => new OrderItemReadModel
                {
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    UnitPrice = item.UnitPrice,
                    Product = new ProductOrderItemReadModel
                    {
                        Id = _hashids.EncodeLong(productsMap[item.ProductId].Id),
                        Name = productsMap[item.ProductId].Name,
                        Price = productsMap[item.ProductId].Price,
                    }
                })
            ]
        };

        await _orders.SyncOrderCreatedAsync(model, cancellationToken);
    }
}