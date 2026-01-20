namespace OrderManagement.Application.Models.Customer;

public record CustomerReadModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }
};