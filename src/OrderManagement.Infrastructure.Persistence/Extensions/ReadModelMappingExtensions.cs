using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Infrastructure.Persistence.Models;

namespace OrderManagement.Infrastructure.Persistence.Extensions;

public static class ReadModelMappingExtensions
{
    extension(Product product)
    {
        public ProductDocumentModel ToReadModel(string hashedId)
        {
            return new ProductDocumentModel { HashedId = hashedId, Name = product.Name, Price = product.Price.ToCents() };
        }
    }
}