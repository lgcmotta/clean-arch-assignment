using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Products;

public sealed class Product() : AggregateRoot
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Money Price { get; private set; }

    public Product(string name, decimal price) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Price = new Money(price);
    }

    public void RaiseProductCreatedDomainEvent()
    {
        AddDomainEvent(new ProductCreatedDomainEvent(Id));
    }
}