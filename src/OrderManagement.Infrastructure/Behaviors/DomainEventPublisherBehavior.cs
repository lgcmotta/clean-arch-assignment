using MediatR;
using OrderManagement.Domain.Core;
using OrderManagement.Infrastructure.Persistence.Contexts;
using OrderManagement.Infrastructure.Persistence.Extensions;
using System.Threading.Channels;

namespace OrderManagement.Infrastructure.Behaviors;

public sealed class DomainEventPublisherBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand
{
    private readonly Channel<IDomainEvent> _channel;
    private readonly AppDbContext _context;

    public DomainEventPublisherBehavior(Channel<IDomainEvent> channel, AppDbContext context)
    {
        _channel = channel;
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        var events = _context.ExtractAndClearDomainEvents();

        foreach (var @event in events)
        {
            await _channel.Writer.WriteAsync(@event, cancellationToken);
        }

        return response;
    }
}