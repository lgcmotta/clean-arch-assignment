using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.Events;

public record OrderCancelledDomainEvent(long Id, long CustomerId) : IDomainEvent;