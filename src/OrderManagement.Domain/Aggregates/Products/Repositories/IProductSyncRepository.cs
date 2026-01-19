namespace OrderManagement.Domain.Aggregates.Products.Repositories;

public interface IProductSyncRepository
{
    Task<Product?> GetProductById(int id, CancellationToken cancellationToken = default);

    ValueTask AddProductAsync(Product product, string hashedId, CancellationToken cancellationToken = default);
}