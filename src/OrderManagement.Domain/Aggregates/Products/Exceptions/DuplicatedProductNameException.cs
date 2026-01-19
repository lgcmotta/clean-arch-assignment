namespace OrderManagement.Domain.Aggregates.Products.Exceptions;

public class DuplicatedProductNameException(string name) : Exception($"A product with name '{name}' already exists on the catalogue.");

public class ProductNotFoundForSyncException(string id) : Exception($"Product with id '{id}' was not found. Sync failed");