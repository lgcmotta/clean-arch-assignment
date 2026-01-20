using HashidsNet;
using MediatR;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Domain.Aggregates.Orders.ValueObjects;

namespace OrderManagement.Application.Commands.Orders.Cancel;

internal sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse>
{
    private readonly IOrderWriteRepository _orderRepository;
    private readonly IHashids _hashids;

    public CancelOrderCommandHandler(IOrderWriteRepository orderRepository, IHashids hashids)
    {
        _orderRepository = orderRepository;
        _hashids = hashids;
    }

    public async Task<CancelOrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var customerId = _hashids.DecodeSingleLong(request.CustomerId);
        var orderId = _hashids.DecodeSingleLong(request.OrderId);

        var order = await _orderRepository.FindCustomerOrderAsync(customerId, orderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(request.OrderId);
        }

        order.Cancel();

        await _orderRepository.SaveChangesAsync(cancellationToken);

        order.RaiseOrderCancelledEvent();

        return new CancelOrderResponse(request.CustomerId, request.OrderId, order.Status.Value);
    }
}