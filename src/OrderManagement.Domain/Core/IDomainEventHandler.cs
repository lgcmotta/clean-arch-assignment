using MediatR;

namespace OrderManagement.Domain.Core;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent;