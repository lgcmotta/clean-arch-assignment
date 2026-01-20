namespace OrderManagement.Domain.Aggregates.Products.Repositories;

public interface IProductWriteRepository
{
    ValueTask<Product?> FindProductAsync(long id, CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<Product>> FindProductsAsync(IEnumerable<long> productIds, CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

    ValueTask AddProductAsync(Product product, CancellationToken cancellationToken = default);

    ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
}