using MongoDB.Driver;
using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Repositories.Customers;
using OrderManagement.Infrastructure.Persistence.Documents.Customers;
using OrderManagement.Infrastructure.Persistence.Mappings.Customers;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Customers;

public class CustomerReadRepository(IMongoClient mongo): ICustomerReadRepository
{
    private readonly IMongoCollection<CustomerDocument> _collection = mongo.GetDatabase("ordering")
        .GetCollection<CustomerDocument>("customers");

    public async ValueTask SyncReadModelAsync(CustomerReadModel model, CancellationToken cancellationToken = default)
    {
        var document = model.ToDocument();

        var filter = Builders<CustomerDocument>.Filter.Eq(doc => doc.ExternalId, document.ExternalId);

        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(filter, document, options, cancellationToken);
    }
}