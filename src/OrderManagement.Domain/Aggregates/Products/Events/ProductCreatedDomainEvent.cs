using OrderManagement.Domain.Core;

namespace OrderManagement.Domain.Aggregates.Products;

public record ProductCreatedDomainEvent(int Id) : IDomainEvent;