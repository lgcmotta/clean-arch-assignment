using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Customers.Events;

public record CustomerCreatedDomainEvent(long CustomerId) : IDomainEvent;