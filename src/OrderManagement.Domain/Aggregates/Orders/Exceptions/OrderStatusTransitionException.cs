namespace OrderManagement.Domain.Aggregates.Orders.Exceptions;

public sealed class OrderStatusTransitionException(string source, string destination)
    : Exception($"Order status cannot change from '{source}' to '{destination}'.");