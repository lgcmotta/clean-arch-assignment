using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Customers;

public sealed class Customer() : AggregateRoot
{
    public Customer(string name, string email, string phone) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        Name = name;
        Email = email;
        Phone = phone;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;
}