using OrderManagement.Application.Models.Products;
using OrderManagement.Domain.Aggregates.Products;

namespace OrderManagement.Application.Extensions;

public static class ReadModelMappingExtensions
{
    extension(Product product)
    {
        internal ProductReadModel ToReadModel(string externalId) =>
            new()
            {
                Id = externalId,
                Name = product.Name,
                Price = product.Price
            };
    }
}