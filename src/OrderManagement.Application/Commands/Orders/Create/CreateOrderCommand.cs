using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Orders.Create;

[UsedImplicitly]
public sealed record CreateOrderItem(string ProductId, int Quantity);

[UsedImplicitly]
public sealed record CreateOrderCommand(string CustomerId, CreateOrderItem[] Items) : IRequest<CreateOrderResponse>, ICommand;

public sealed record CreateOrderResponse(string Id, string CustomerId, string Status);