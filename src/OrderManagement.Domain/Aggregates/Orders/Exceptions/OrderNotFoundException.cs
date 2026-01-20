namespace OrderManagement.Domain.Aggregates.Orders.Exceptions;

public sealed class OrderNotFoundException : Exception
{
    public OrderNotFoundException(string id) : base($"Order with id '{id}' was not found.") { }
}
