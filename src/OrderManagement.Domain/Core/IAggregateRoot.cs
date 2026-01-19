namespace OrderManagement.Domain.Core;

public interface IAggregateRoot
{
    IEnumerable<IDomainEvent> Events { get; }

    public void ClearDomainEvents();
}