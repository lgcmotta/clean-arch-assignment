using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.ValueObjects;

public record OrderStatus : Enumeration
{
    private OrderStatus(int key, string value) : base(key, value)
    {
    }

    public static OrderStatus Created => new(0, nameof(Created));

    public static OrderStatus PendingPayment => new(1, nameof(PendingPayment));

    public static OrderStatus PaymentApproved => new(2, nameof(PaymentApproved));

    public static OrderStatus PaymentRejected => new(3, nameof(PaymentRejected));

    public static OrderStatus PendingShipment => new(4, nameof(PendingShipment));

    public static OrderStatus Shipped => new(5, nameof(Shipped));

    public static OrderStatus InTransit => new(6, nameof(InTransit));

    public static OrderStatus Delivered => new(7, nameof(Delivered));

    public static OrderStatus Returned => new(8, nameof(Returned));

    public static OrderStatus Cancelled => new(9, nameof(Cancelled));
}