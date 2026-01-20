using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Documents.Customers;

public sealed record CustomerDocument
{
    [BsonId, BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("external_id")]
    public required string ExternalId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("email")]
    public required string Email { get; set; }

    [BsonElement("phone")]
    public required string Phone { get; set; }
}