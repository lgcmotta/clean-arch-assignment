using MongoDB.Bson;
using OrderManagement.Application.Models.Orders;
using OrderManagement.Infrastructure.Persistence.Documents.Orders;

namespace OrderManagement.Infrastructure.Persistence.Mappings.Orders;

public static class OrderDocumentMapping
{
    extension(OrderDocument document)
    {
        internal OrderReadModel ToReadModel()
        {
            return new OrderReadModel
            {
                Id = document.ExternalId,
                CreatedDate = document.CreatedDate,
                TotalAmount = document.TotalAmount / 100m,
                Status = document.Status,
                Customer = new CustomerOrderReadModel
                {
                    Id = document.Customer.Id,
                    Email = document.Customer.Email,
                    Name = document.Customer.Name,
                    Phone = document.Customer.Phone,
                },
                Items =
                [
                    ..document.Items.Select(item => item.ToReadModel())
                ],

            };
        }
    }

    extension(OrderReadModel model)
    {
        internal OrderDocument ToDocument()
        {
            return new OrderDocument
            {
                Id = new ObjectId(),
                ExternalId = model.Id,
                CreatedDate = model.CreatedDate,
                TotalAmount = decimal.ToInt64(model.TotalAmount * 100m),
                Status = model.Status,
                Customer = new CustomerOrderDocument
                {
                    Id = model.Customer.Id,
                    Email = model.Customer.Email,
                    Name = model.Customer.Name,
                    Phone = model.Customer.Phone,
                },
                Items =
                [
                    ..model.Items.Select(item => item.ToDocument())
                ]
            };
        }
    }

    extension(OrderItemDocument document)
    {
        private OrderItemReadModel ToReadModel() =>
            new()
            {
                Quantity = document.Quantity,
                TotalPrice = document.TotalPrice / 100m,
                UnitPrice = document.UnitPrice / 100m,
                Product = document.Product.ToReadModel()
            };
    }

    extension(OrderItemReadModel model)
    {
        private OrderItemDocument ToDocument() =>
            new()
            {
                Quantity = model.Quantity,
                TotalPrice = decimal.ToInt64(model.TotalPrice * 100m),
                UnitPrice = decimal.ToInt64(model.UnitPrice * 100m),
                Product = model.Product.ToDocument()
            };
    }

    extension(ProductOrderItemDocument document)
    {
        private ProductOrderItemReadModel ToReadModel() =>
            new()
            {
                Id = document.Id,
                Name = document.Name,
                Price = document.Price / 100m
            };
    }

    extension(ProductOrderItemReadModel model)
    {
        private ProductOrderItemDocument ToDocument() =>
            new()
            {
                Id = model.Id,
                Name = model.Name,
                Price = decimal.ToInt64(model.Price * 100m)
            };
    }
}