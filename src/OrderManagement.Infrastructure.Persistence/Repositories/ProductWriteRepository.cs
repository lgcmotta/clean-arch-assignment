using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Infrastructure.Persistence.Contexts;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public class ProductWriteRepository(SqlServerDbContext context) : IProductWriteRepository
{
    public async ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default) =>
        await context.Set<Product>().AsNoTracking().AnyAsync(product => product.Name == name, cancellationToken);

    public async ValueTask AddProductAsync(Product product, CancellationToken cancellationToken = default) =>
        await context.Set<Product>().AddAsync(product, cancellationToken);

    public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}