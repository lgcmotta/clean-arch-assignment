using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Models;

public sealed class OrderItemModel
{
    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unit_price")]
    public long UnitPrice { get; set; }

    [BsonElement("total_price")]
    public long TotalPrice { get; set; }

    [BsonElement("product")]
    public required ProductModel Product { get; set; }
}