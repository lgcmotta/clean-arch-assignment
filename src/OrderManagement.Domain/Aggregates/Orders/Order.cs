using OrderManagement.Domain.Aggregates.Orders.Entities;
using OrderManagement.Domain.Aggregates.Orders.Events;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.ValueObjects;
using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders;

public sealed class Order() : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    public long Id { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public long CustomerId { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Created;

    public Money TotalAmount => _items.Aggregate(Money.Zero, (money, item) => money + item.TotalPrice);

    public IEnumerable<OrderItem> Items => _items.AsReadOnly();

    public Order(long customerId, DateTimeOffset createdDate) : this()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);

        CustomerId = customerId;
        CreatedDate = createdDate;
    }

    public void ChangeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return;
        }

        var orderStatus = Enumeration.ParseByValue<OrderStatus>(status);

        var transition = OrderStatus.Transitions[Status];

        if (!transition.Contains(orderStatus))
        {
            throw new OrderStatusTransitionException(Status.Value, orderStatus.Value);
        }

        if (orderStatus == Status)
        {
            return;
        }

        Status = orderStatus;
    }

    public void Cancel()
    {
        var transition = OrderStatus.Transitions[Status];

        if (!transition.Contains(OrderStatus.Cancelled))
        {
            throw new OrderStatusTransitionException(Status.Value, OrderStatus.Cancelled.Value);
        }

        Status = OrderStatus.Cancelled;
    }

    public void IncludeItem(long productId, string productName, int quantity, Money unitPrice)
    {
        var item = _items.SingleOrDefault(i => i.ProductId == productId);

        if (item is not null)
        {
            item.UpdateUnitPriceIfChanged(unitPrice);

            item.IncreaseQuantityBy(quantity);

            item.RecalculateTotalPrice();

            return;
        }

        item = new OrderItem(productId, productName, quantity, unitPrice);

        item.RecalculateTotalPrice();

        _items.Add(item);
    }

    public void SetItemQuantity(long productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
        {
            RemoveItem(productId);
            return;
        }

        var item = _items.SingleOrDefault(item => item.ProductId == productId);

        if (item is null)
        {
            IncludeItem(productId, productName, quantity, unitPrice);
            return;
        }

        item.UpdateDetails(productName, unitPrice);
        item.SetQuantity(quantity);
        item.RecalculateTotalPrice();
    }

    public void RemoveItem(long productId)
    {
        var item = _items.SingleOrDefault(item => item.ProductId == productId);

        if (item is not null)
        {
            _items.Remove(item);
        }
    }

    public void RaiseOrderCreatedEvent()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(Id, CustomerId));
    }

    public void RaiseOrderUpdatedEvent()
    {
        AddDomainEvent(new OrderUpdatedDomainEvent(Id, CustomerId));
    }

    public void RaiseOrderCancelledEvent()
    {
        AddDomainEvent(new OrderCancelledDomainEvent(Id, CustomerId));
    }
}