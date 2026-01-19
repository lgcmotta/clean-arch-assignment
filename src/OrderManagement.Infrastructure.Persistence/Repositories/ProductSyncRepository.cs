using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Infrastructure.Persistence.Contexts;
using OrderManagement.Infrastructure.Persistence.Extensions;
using OrderManagement.Infrastructure.Persistence.Models;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public sealed class ProductSyncRepository : IProductSyncRepository
{
    private readonly SqlServerDbContext _sqlServer;
    private readonly IMongoClient _mongo;

    public ProductSyncRepository(SqlServerDbContext sqlServer, IMongoClient mongo)
    {
        _sqlServer = sqlServer;
        _mongo = mongo;
    }

    public async Task<Product?> GetProductById(int id, CancellationToken cancellationToken = default) =>
        await _sqlServer.Set<Product>().AsNoTracking().SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async ValueTask AddProductAsync(Product product, string hashedId, CancellationToken cancellationToken = default)
    {
        var database = _mongo.GetDatabase("ordering");

        var collection = database.GetCollection<ProductDocumentModel>("products");

        var model = product.ToReadModel(hashedId);

        await collection.InsertOneAsync(model, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);
    }
}