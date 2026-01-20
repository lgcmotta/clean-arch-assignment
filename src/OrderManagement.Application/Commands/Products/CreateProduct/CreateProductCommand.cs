using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Products.CreateProduct;

[UsedImplicitly]
public record CreateProductCommand(string Name, decimal Price) : IRequest<CreateProductResponse>, ICommand;

public record CreateProductResponse(string Id, string Name, decimal Price);