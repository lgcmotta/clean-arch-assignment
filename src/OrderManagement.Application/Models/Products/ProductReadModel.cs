namespace OrderManagement.Application.Models.Products;

public record ProductReadModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required decimal Price { get; init; }
}