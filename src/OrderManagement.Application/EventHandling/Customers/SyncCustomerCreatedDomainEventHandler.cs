using HashidsNet;
using OrderManagement.Application.Extensions;
using OrderManagement.Application.Repositories.Customers;
using OrderManagement.Domain.Aggregates.Customers.Events;
using OrderManagement.Domain.Aggregates.Customers.Exceptions;
using OrderManagement.Domain.Aggregates.Customers.Repositories;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.EventHandling.Customers;

internal sealed class SyncCustomerCreatedDomainEventHandler : IDomainEventHandler<CustomerCreatedDomainEvent>
{
    private readonly ICustomerWriteRepository _writer;
    private readonly ICustomerReadRepository _reader;
    private readonly IHashids _hashids;

    public SyncCustomerCreatedDomainEventHandler(ICustomerWriteRepository writer, ICustomerReadRepository reader, IHashids hashids)
    {
        _writer = writer;
        _reader = reader;
        _hashids = hashids;
    }
    public async Task Handle(CustomerCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var customer = await _writer.FindCustomerByIdAsync(notification.CustomerId, cancellationToken);

        if (customer is null)
        {
            throw new CustomerNotFoundException(_hashids.EncodeLong(notification.CustomerId));
        }

        var customerId = _hashids.EncodeLong(customer.Id);

        var model = customer.ToReadModel(customerId);

        await _reader.SyncReadModelAsync(model, cancellationToken);
    }
}