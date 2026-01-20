using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Orders.Update;

[UsedImplicitly]
public record UpdateOrderItem(string ProductId, int Quantity);

[UsedImplicitly]
public record UpdateOrderCommand(string CustomerId, string OrderId, string? Status = null, UpdateOrderItem[]? Items = null) : IRequest<UpdateOrderResponse>, ICommand;

[UsedImplicitly]
public record UpdateOrderResponse(string Id, string CustomerId, string Status);