using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.Events;

public record OrderCreatedDomainEvent(long OrderId, long CustomerId) : IDomainEvent;