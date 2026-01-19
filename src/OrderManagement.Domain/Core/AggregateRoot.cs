namespace OrderManagement.Domain.Core;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IEnumerable<IDomainEvent> Events => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    protected void RemoveDomainEvent(IDomainEvent @event) => _domainEvents.Remove(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}