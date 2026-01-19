using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Infrastructure.Persistence.Models;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public class ProductReadRepository(IMongoClient mongo) : IProductReadRepository<ProductDocumentModel>
{
    public async ValueTask<IEnumerable<ProductDocumentModel>> GetProductsAsync(int page, int size, string? sortBy, string sort, CancellationToken cancellationToken = default)
    {
        var database = mongo.GetDatabase("ordering");

        var collection = database.GetCollection<ProductDocumentModel>("products");

        var queryable = collection.AsQueryable();

        queryable = sortBy switch
        {
            "price" when sort is "DESC" => queryable.OrderByDescending(product => product.Price),
            "price" when sort is "ASC" => queryable.OrderBy(product => product.Price),
            "name" when sort is "DESC" => queryable.OrderByDescending(product => product.Name),
            _ => queryable.OrderBy(product => product.Name),
        };

        var products = await queryable.Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return products;
    }

    public async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
    {
        var database = mongo.GetDatabase("ordering");

        var collection = database.GetCollection<ProductDocumentModel>("products");

        return await collection.CountDocumentsAsync(FilterDefinition<ProductDocumentModel>.Empty, cancellationToken: cancellationToken);
    }
}