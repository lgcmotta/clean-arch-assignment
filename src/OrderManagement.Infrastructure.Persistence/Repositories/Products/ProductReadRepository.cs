using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OrderManagement.Application.Models.Products;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Products;
using OrderManagement.Infrastructure.Persistence.Documents.Products;
using OrderManagement.Infrastructure.Persistence.Extensions;
using OrderManagement.Infrastructure.Persistence.Mappings.Products;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Products;

public class ProductReadRepository(IMongoClient mongo) : IProductReadRepository
{
    private readonly IMongoCollection<ProductDocument> _collection = mongo.GetDatabase("ordering")
        .GetCollection<ProductDocument>("products");

    public async ValueTask<(IEnumerable<ProductReadModel>, PagedResponseModel)> PaginateProductsAsync(
        long total,
        PagedQueryModel query,
        CancellationToken cancellationToken = default)
    {
        var sortBy = query.SortBy?.Trim().ToLowerInvariant();

        var queryable = _collection.AsQueryable();

        queryable = sortBy switch
        {
            "price" when query.Sort is SortingOrder.DESC => queryable.OrderByDescending(product => product.Price),
            "price" when query.Sort is SortingOrder.ASC => queryable.OrderBy(product => product.Price),
            "name" when query.Sort is SortingOrder.DESC => queryable.OrderByDescending(product => product.Name),
            _ => queryable.OrderBy(product => product.Name)
        };

        var pagination = query.ToPagedResponse(total);

        var documents = await queryable.Skip((pagination.Page - 1) * pagination.Size)
            .Take(pagination.Size)
            .ToListAsync(cancellationToken);

        var models = documents.Select(document => document.ToReadModel());

        return (models, pagination);
    }

    public async ValueTask<long> CountTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<ProductDocument>.Empty, cancellationToken: cancellationToken);
    }

    public async ValueTask SyncReadModelAsync(ProductReadModel model, CancellationToken cancellationToken = default)
    {
        var document = model.ToDocument();

        var filter = Builders<ProductDocument>.Filter.Eq(doc => doc.ExternalId, document.ExternalId);

        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(filter, document, options, cancellationToken);
    }
}