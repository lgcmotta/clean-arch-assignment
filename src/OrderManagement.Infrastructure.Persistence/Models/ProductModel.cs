using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Models;

public class ProductModel
{
    [BsonElement("id")]
    public required string Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("price")]
    public required long Price { get; set; }
}