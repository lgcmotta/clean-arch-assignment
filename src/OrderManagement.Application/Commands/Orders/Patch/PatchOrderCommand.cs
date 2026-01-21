using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Orders.Patch;

[UsedImplicitly]
public record PatchOrderItem(string ProductId, int Quantity);

[UsedImplicitly]
public record PatchOrderCommand(string CustomerId, string OrderId, string? Status = null, PatchOrderItem[]? Items = null) : IRequest<PatchOrderResponse>, ICommand;

[UsedImplicitly]
public record PatchOrderResponse(string Id, string CustomerId, string Status, PatchOrderItemResponse[] Items);

[UsedImplicitly]
public record PatchOrderItemResponse
{
    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string ProductId { get; init; }

    public required string ProductName { get; init; }
}