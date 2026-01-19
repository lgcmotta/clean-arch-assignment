using OrderManagement.Domain.Aggregates.Orders.Entities;
using OrderManagement.Domain.Aggregates.Orders.Events;
using OrderManagement.Domain.Aggregates.Orders.ValueObjects;
using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    public int Id { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public int CustomerId { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Created;

    public Money TotalAmount => _items.Aggregate(Money.Zero, (money, item) => money + item.TotalPrice);

    public IEnumerable<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        if (_items.Any(item => item.ProductId == productId))
        {
            throw new Exception(); // TODO: Change to domain exception
        }

        var orderItem = new OrderItem(productId, productName, quantity, unitPrice);

        _items.Add(orderItem);
    }

    public void RaiseOrderCreatedEvent()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(Id));
    }

}