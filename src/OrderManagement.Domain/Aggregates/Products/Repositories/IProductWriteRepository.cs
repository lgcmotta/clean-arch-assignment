namespace OrderManagement.Domain.Aggregates.Products.Repositories;

public interface IProductWriteRepository
{
    ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    ValueTask AddProductAsync(Product product, CancellationToken cancellationToken = default);
    ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
}