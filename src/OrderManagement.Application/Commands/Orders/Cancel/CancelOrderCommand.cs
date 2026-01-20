using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Orders.Cancel;

[UsedImplicitly]
public record CancelOrderCommand(string CustomerId, string OrderId) : IRequest<CancelOrderResponse>, ICommand;

public record CancelOrderResponse(string CustomerId, string Id, string Status);