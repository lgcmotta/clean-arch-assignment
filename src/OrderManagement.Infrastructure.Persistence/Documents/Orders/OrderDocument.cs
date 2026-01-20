using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Documents.Orders;

public sealed record OrderDocument
{
    [BsonId, BsonElement("_id")]
    public ObjectId Id { get; init; }

    [BsonElement("external_id")]
    public required string ExternalId { get; init; } = string.Empty;

    [BsonElement("customer")]
    public required CustomerOrderDocument Customer { get; init; }

    [BsonElement("order_date")]
    public required DateTimeOffset CreatedDate { get; init; }

    [BsonElement("total_amount")]
    public required long TotalAmount { get; init; }

    [BsonElement("status")]
    public required string Status { get; init; } = string.Empty;

    [BsonElement("items")]
    public required List<OrderItemDocument> Items { get; init; } = [];
}

public sealed record CustomerOrderDocument
{
    [BsonElement("id")]
    public required string Id { get; init; } = string.Empty;

    [BsonElement("name")]
    public required string Name { get; init; } = string.Empty;

    [BsonElement("email")]
    public required string Email { get; init; } = string.Empty;

    [BsonElement("phone")]
    public required string Phone { get; init; } = string.Empty;
}

public sealed record OrderItemDocument
{
    [BsonElement("quantity")]
    public int Quantity { get; init; }

    [BsonElement("unit_price")]
    public long UnitPrice { get; init; }

    [BsonElement("total_price")]
    public long TotalPrice { get; init; }

    [BsonElement("product")]
    public required ProductOrderItemDocument Product { get; init; }
}

public sealed record ProductOrderItemDocument
{
    [BsonElement("id")]
    public required string Id { get; init; }

    [BsonElement("name")]
    public required string Name { get; init; }

    [BsonElement("price")]
    public required long Price { get; init; }
}