using MongoDB.Bson;
using OrderManagement.Application.Models.Customer;
using OrderManagement.Infrastructure.Persistence.Documents.Customers;

namespace OrderManagement.Infrastructure.Persistence.Mappings.Customers;

public static class CustomerDocumentMapping
{
    extension(CustomerDocument document)
    {
        internal CustomerReadModel ToReadModel() =>
            new()
            {
                Id = document.ExternalId,
                Name = document.Name,
                Email = document.Email,
                Phone = document.Phone
            };
    }

    extension(CustomerReadModel model)
    {
        internal CustomerDocument ToDocument(ObjectId? objectId = null) =>
            new()
            {
                Id = (objectId is null || objectId == ObjectId.Empty ? ObjectId.GenerateNewId() : objectId.Value),
                ExternalId = model.Id,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone
            };
    }
}