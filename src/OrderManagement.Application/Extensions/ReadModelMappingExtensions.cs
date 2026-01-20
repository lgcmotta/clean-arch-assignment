using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Models.Products;
using OrderManagement.Domain.Aggregates.Customers;
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

    extension(Customer customer)
    {
        internal CustomerReadModel ToReadModel(string externalId) =>
            new()
            {
                Id = externalId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone
            };
    }
}