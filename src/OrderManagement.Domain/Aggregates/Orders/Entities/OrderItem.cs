using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.Entities;

public sealed class OrderItem()
{
    public OrderItem(long productId, string productName, int quantity, Money unitPrice) : this()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice.Value);

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public long ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money TotalPrice { get; private set; }

    public void IncreaseQuantityBy(int value)
    {
        Quantity += value;


    }

    public void DecreaseQuantityBy(int value)
    {
        Quantity -= value;

        if (Quantity < 0)
        {
            Quantity = 0;
        }
    }

    public void SetQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        Quantity = quantity;
    }

    public void UpdateDetails(string productName, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice);

        ProductName = productName;
        UnitPrice = unitPrice;
        RecalculateTotalPrice();
    }

    public void RecalculateTotalPrice()
    {
        TotalPrice = UnitPrice * Quantity;
    }

    public void UpdateUnitPriceIfChanged(Money unitPrice)
    {
        if (unitPrice != UnitPrice)
        {
            UnitPrice = unitPrice;
        }
    }
}