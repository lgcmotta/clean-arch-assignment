using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Models;

public sealed class OrderDocumentModel
{
    [BsonId, BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("hashed_id")]
    public required string HashedId { get; set; } = string.Empty;

    [BsonElement("customer")]
    public required CustomerModel Customer { get; set; }

    [BsonElement("order_date")]
    public required DateTimeOffset OrderDate { get; set; }

    [BsonElement("status")]
    public required string Status { get; set; } = string.Empty;

    [BsonElement("items")]
    public required List<OrderItemModel> Items { get; set; } = [];
}