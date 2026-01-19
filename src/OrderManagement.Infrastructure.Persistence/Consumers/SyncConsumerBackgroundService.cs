using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderManagement.Domain.Core;
using System.Threading.Channels;

namespace OrderManagement.Infrastructure.Persistence.Consumers;

public class SyncConsumerBackgroundService : BackgroundService
{
    private readonly Channel<IDomainEvent> _channel;
    private readonly IServiceScopeFactory _factory;

    public SyncConsumerBackgroundService(Channel<IDomainEvent> channel, IServiceScopeFactory factory)
    {
        _channel = channel;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await using var scope = _factory.CreateAsyncScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(@event, stoppingToken);
        }
    }
}