using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Infrastructure.Persistence.Models;

public sealed class CustomerModel
{
    [BsonElement("id")]
    public required string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public required string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public required string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public required string Phone { get; set; } = string.Empty;
}