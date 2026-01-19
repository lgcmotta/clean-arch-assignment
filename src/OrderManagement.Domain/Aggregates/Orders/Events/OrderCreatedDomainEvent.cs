using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Orders.Events;

public record OrderCreatedDomainEvent(int Id) : IDomainEvent;