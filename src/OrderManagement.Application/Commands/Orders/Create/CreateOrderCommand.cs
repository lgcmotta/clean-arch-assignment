using HashidsNet;
using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Aggregates.Orders;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Orders.Create;

public record CreateOrderItem(string ProductId, int Quantity);

[UsedImplicitly]
public record CreateOrderCommand(string CustomerId, CreateOrderItem[] Items) : IRequest<CreateOrderResponse>, ICommand;

public record CreateOrderResponse(string Id, string CustomerId, string Status);