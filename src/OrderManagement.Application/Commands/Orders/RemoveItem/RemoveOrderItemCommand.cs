using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Orders.RemoveItem;

[UsedImplicitly]
public record RemoveOrderItemCommand(string CustomerId, string OrderId, string ProductId)
    : IRequest<OrderItemRemovedResponse>, ICommand;

[UsedImplicitly]
public record OrderItemRemovedResponse(string Id, string CustomerId, string Status, RemoveOrderItemResponse[] Items);

[UsedImplicitly]
public record RemoveOrderItemResponse
{
    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string ProductId { get; init; }

    public required string ProductName { get; init; }
}