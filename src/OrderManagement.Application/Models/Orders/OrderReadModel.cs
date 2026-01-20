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
    public required string Id { get; set; } = string.Empty;

    public required string Name { get; set; } = string.Empty;

    public required string Email { get; set; } = string.Empty;

    public required string Phone { get; set; } = string.Empty;
}

public sealed record OrderItemReadModel
{
    public int Quantity { get; set; }

    public required decimal UnitPrice { get; set; }

    public required decimal TotalPrice { get; set; }

    public required ProductOrderItemReadModel Product { get; set; }
}

public sealed record ProductOrderItemReadModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required decimal Price { get; set; }
}