using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Customers;
using OrderManagement.Infrastructure.Persistence.Documents.Customers;
using OrderManagement.Infrastructure.Persistence.Extensions;
using OrderManagement.Infrastructure.Persistence.Mappings.Customers;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Customers;

public class CustomerReadRepository(IMongoClient mongo) : ICustomerReadRepository
{
    private readonly IMongoCollection<CustomerDocument> _collection = mongo.GetDatabase("ordering")
        .GetCollection<CustomerDocument>("customers");

    public async ValueTask<(IEnumerable<CustomerReadModel>, PagedResponseModel)> PaginateCustomersAsync(long total, PagedQueryModel query, CancellationToken cancellationToken = default)
    {
        var sortBy = query.SortBy?.Trim().ToLowerInvariant();

        var queryable = _collection.AsQueryable();

        queryable = sortBy switch
        {
            "email" when query.Sort is SortingOrder.DESC => queryable.OrderByDescending(customer => customer.Email),
            "email" when query.Sort is SortingOrder.ASC => queryable.OrderBy(customer => customer.Email),
            "phone" when query.Sort is SortingOrder.DESC => queryable.OrderByDescending(customer => customer.Phone),
            "phone" when query.Sort is SortingOrder.ASC => queryable.OrderBy(customer => customer.Phone),
            "name" when query.Sort is SortingOrder.DESC => queryable.OrderByDescending(customer => customer.Name),
            _ => queryable.OrderBy(customer => customer.Name)
        };

        var pagination = query.ToPagedResponse(total);

        var documents = await queryable.Skip((pagination.Page - 1) * pagination.Size)
            .Take(pagination.Size)
            .ToListAsync(cancellationToken);

        var models = documents.Select(document => document.ToReadModel());

        return (models, pagination);
    }

    public async ValueTask<long> CountTotalCustomersAsync(CancellationToken cancellationToken = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<CustomerDocument>.Empty, cancellationToken: cancellationToken);

    public async ValueTask SyncReadModelAsync(CustomerReadModel model, CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerDocument>.Filter.Eq(doc => doc.ExternalId, model.Id);

        var objectId = await _collection.Find(filter).Project(document => document.Id).SingleOrDefaultAsync(cancellationToken);

        var document = model.ToDocument(objectId);

        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(filter, document, options, cancellationToken);
    }
}