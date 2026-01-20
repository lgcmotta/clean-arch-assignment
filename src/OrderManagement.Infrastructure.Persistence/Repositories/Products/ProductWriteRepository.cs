using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Infrastructure.Persistence.Contexts;
using System.Security.Principal;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Products;

public class ProductWriteRepository(AppDbContext context) : IProductWriteRepository
{
    public async ValueTask<Product?> FindProductAsync(long id, CancellationToken cancellationToken = default) =>
        await context.Set<Product>().AsNoTracking().SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async ValueTask<IEnumerable<Product>> FindProductsAsync(IEnumerable<long> productIds, CancellationToken cancellationToken = default) =>
        await context.Set<Product>().AsNoTracking().Where(product => productIds.Contains(product.Id)).ToListAsync(cancellationToken);

    public async ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default) =>
        await context.Set<Product>().AsNoTracking().AnyAsync(product => product.Name == name, cancellationToken);

    public async ValueTask AddProductAsync(Product product, CancellationToken cancellationToken = default) =>
        await context.Set<Product>().AddAsync(product, cancellationToken);

    public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}