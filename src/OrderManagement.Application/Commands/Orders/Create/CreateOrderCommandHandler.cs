using HashidsNet;
using MediatR;
using OrderManagement.Domain.Aggregates.Customers.Exceptions;
using OrderManagement.Domain.Aggregates.Customers.Repositories;
using OrderManagement.Domain.Aggregates.Orders;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Domain.Aggregates.Products.Repositories;

namespace OrderManagement.Application.Commands.Orders.Create;

internal sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderWriteRepository _orders;
    private readonly ICustomerWriteRepository _customers;
    private readonly IProductWriteRepository _products;
    private readonly IHashids _hashids;
    private readonly TimeProvider _timeProvider;

    public CreateOrderCommandHandler(IOrderWriteRepository orders,
        ICustomerWriteRepository customers,
        IProductWriteRepository products,
        IHashids hashids,
        TimeProvider timeProvider)
    {
        _orders = orders;
        _customers = customers;
        _products = products;
        _hashids = hashids;
        _timeProvider = timeProvider;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customerId = _hashids.DecodeSingleLong(request.CustomerId);

        var customer = await _customers.FindCustomerByIdAsync(customerId, cancellationToken);

        if (customer is null)
        {
            throw new CustomerNotFoundException(request.CustomerId);
        }

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

        var order = new Order(customer.Id, _timeProvider.GetUtcNow());

        foreach (var item in request.Items)
        {
            var productId = _hashids.DecodeSingleLong(item.ProductId);

            if (!productsMap.TryGetValue(productId, out var product))
            {
                throw new ProductNotFoundException(item.ProductId);
            }

            order.IncludeItem(product.Id, product.Name, item.Quantity, product.Price);
        }

        await _orders.AddOrderAsync(order, cancellationToken);

        await _orders.SaveChangesAsync(cancellationToken);

        order.RaiseOrderCreatedEvent();

        var orderId = _hashids.EncodeLong(order.Id);

        return new CreateOrderResponse(orderId, request.CustomerId, order.Status.Value);
    }
}