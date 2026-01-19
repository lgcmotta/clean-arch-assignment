using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.Entities;

public sealed class OrderItem()
{
    public OrderItem(int productId, string productName, int quantity, decimal unitPrice) : this()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice);

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money TotalPrice { get; private set; }

    public void IncreaseQuantityBy(int value)
    {
        Quantity += value;

        RecalculateTotalPrice();
    }

    public void DecreaseQuantityBy(int value)
    {
        Quantity += value;

        if (Quantity < 0)
        {
            Quantity = 0;
        }

        RecalculateTotalPrice();
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = UnitPrice * Quantity;
    }
}