using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OrderManagement.Application.Models.Orders;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Orders;
using OrderManagement.Domain.Aggregates.Orders.ValueObjects;
using OrderManagement.Infrastructure.Persistence.Documents.Orders;
using OrderManagement.Infrastructure.Persistence.Extensions;
using OrderManagement.Infrastructure.Persistence.Mappings.Orders;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Orders;

public class OrderReadRepository(IMongoClient mongo) : IOrderReadRepository
{
    private readonly IMongoCollection<OrderDocument> _collection = mongo.GetDatabase("ordering")
        .GetCollection<OrderDocument>("orders");

    public async ValueTask<(IEnumerable<OrderReadModel>, PagedResponseModel)> GetOrdersByCustomerAsync(string customerId, long total, PagedQueryModel query,
        CancellationToken cancellationToken = default)
    {
        var sortBy = query.SortBy?.Trim().ToLowerInvariant();

        var queryable = _collection.AsQueryable();

        queryable = sortBy switch
        {
            "status" when query.Sort is SortingOrder.DESC => queryable.OrderByDescending(order => order.Status),
            "status" when query.Sort is SortingOrder.ASC => queryable.OrderBy(order => order.Status),
            "order_date" when query.Sort is SortingOrder.ASC => queryable.OrderBy(order => order.CreatedDate),
            _ => queryable.OrderByDescending(order => order.CreatedDate),
        };

        var pagination = query.ToPagedResponse(total);

        var documents = await queryable.Skip((pagination.Page - 1) * pagination.Size)
            .Take(pagination.Size)
            .ToListAsync(cancellationToken);

        var models = documents.Select(document => document.ToReadModel());

        return (models, pagination);
    }

    public async ValueTask<long> CountByCustomerAsync(string customerId, CancellationToken cancellationToken = default) =>
        await _collection.CountDocumentsAsync(order => order.Customer.Id == customerId, cancellationToken: cancellationToken);

    public async ValueTask SyncOrderCancelledAsync(string customerId, string orderId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDocument>.Filter.And(filters:
            [
                Builders<OrderDocument>.Filter.Eq(order => order.Customer.Id, customerId),
                Builders<OrderDocument>.Filter.Eq(order => order.ExternalId, orderId)
            ]
        );

        var update = Builders<OrderDocument>.Update.Set(order => order.Status, OrderStatus.Cancelled.Value);

        await _collection.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async ValueTask SyncOrderCreatedAsync(OrderReadModel model, CancellationToken cancellationToken = default)
    {
        var document = model.ToDocument();

        var filter = Builders<OrderDocument>.Filter.Eq(doc => doc.ExternalId, document.ExternalId);

        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(filter, document, options, cancellationToken);
    }

    public async ValueTask SyncOrderUpdatedAsync(OrderReadModel model, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(doc => doc.ExternalId, model.Id);

        var objectId = await _collection.Find(filter).Project(document => document.Id).SingleOrDefaultAsync(cancellationToken);

        var document = model.ToDocument(objectId);

        var options = new FindOneAndReplaceOptions<OrderDocument> { IsUpsert = false };

        await _collection.FindOneAndReplaceAsync(filter, document, options, cancellationToken);
    }
}