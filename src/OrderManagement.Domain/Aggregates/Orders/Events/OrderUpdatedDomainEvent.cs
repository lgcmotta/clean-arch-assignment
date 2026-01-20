using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.Events;

public record OrderUpdatedDomainEvent(long OrderId, long CustomerId) : IDomainEvent;