using MongoDB.Bson;
using OrderManagement.Application.Models.Products;
using OrderManagement.Infrastructure.Persistence.Documents.Products;

namespace OrderManagement.Infrastructure.Persistence.Mappings.Products;

public static class ProductDocumentMapping
{
    extension(ProductDocument document)
    {
        internal ProductReadModel ToReadModel() =>
            new()
            {
                Id = document.ExternalId,
                Name = document.Name,
                Price = document.Price / 100m
            };
    }

    extension(ProductReadModel model)
    {
        internal ProductDocument ToDocument(ObjectId? objectId = null) =>
            new()
            {
                Id = objectId ?? ObjectId.GenerateNewId(),
                ExternalId = model.Id,
                Name = model.Name,
                Price = decimal.ToInt64(model.Price * 100m)
            };
    }
}