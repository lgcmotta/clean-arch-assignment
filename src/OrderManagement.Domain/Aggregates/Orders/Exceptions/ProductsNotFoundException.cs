namespace OrderManagement.Domain.Aggregates.Orders.Exceptions;

public sealed class ProductsNotFoundException(string[] productIds) : Exception($"Products with ids: {string.Join(',', productIds)} were not found.");

public sealed class ProductNotFoundException(string productId) : Exception($"Product with id '{productId}' was not found.");