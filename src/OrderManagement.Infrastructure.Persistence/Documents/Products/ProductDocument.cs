using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Documents.Products;

public class ProductDocument
{
    [BsonId, BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("external_id")]
    public required string ExternalId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("price")]
    public required long Price { get; set; }
}