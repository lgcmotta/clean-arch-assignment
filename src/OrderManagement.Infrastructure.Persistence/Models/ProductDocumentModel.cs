using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Models;

public class ProductDocumentModel
{
    [BsonId, BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("hashed_id")]
    public required string HashedId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("price")]
    public required long Price { get; set; }
}