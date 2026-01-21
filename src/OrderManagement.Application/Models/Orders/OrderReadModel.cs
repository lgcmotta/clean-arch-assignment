namespace OrderManagement.Application.Models.Orders;

public sealed record OrderReadModel
{
    public required string Id { get; init; }

    public required CustomerOrderReadModel Customer { get; init; }

    public required DateTimeOffset CreatedDate { get; init; }

    public required decimal TotalAmount { get; init; }

    public required string Status { get; init; }

    public required List<OrderItemReadModel> Items { get; init; } = [];
}

public sealed record CustomerOrderReadModel
{
    public required string Id { get; init; } = string.Empty;

    public required string Name { get; init; } = string.Empty;

    public required string Email { get; init; } = string.Empty;

    public required string Phone { get; init; } = string.Empty;
}

public sealed record OrderItemReadModel
{
    public int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public required decimal TotalPrice { get; init; }

    public required ProductOrderItemReadModel Product { get; init; }
}

public sealed record ProductOrderItemReadModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required decimal Price { get; init; }
}