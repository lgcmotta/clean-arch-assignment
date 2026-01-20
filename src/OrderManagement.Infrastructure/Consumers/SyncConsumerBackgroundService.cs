using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Core;
using System.Diagnostics;
using System.Threading.Channels;

namespace OrderManagement.Infrastructure.Consumers;

public class SyncConsumerBackgroundService : BackgroundService
{
    private static readonly ActivitySource Source = new("OrderManagement.Infrastructure", "1.0.0");

    private readonly ILogger<SyncConsumerBackgroundService> _logger;
    private readonly Channel<IDomainEvent> _channel;
    private readonly IServiceScopeFactory _factory;

    public SyncConsumerBackgroundService(ILogger<SyncConsumerBackgroundService> logger, Channel<IDomainEvent> channel, IServiceScopeFactory factory)
    {
        _logger = logger;
        _channel = channel;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var activity = Source.StartActivity(nameof(ExecuteAsync), ActivityKind.Consumer);

                activity?.SetTag("domain_event", @event);

                await using var scope = _factory.CreateAsyncScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Publish(@event, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An exception occurred while consuming domain events");
        }
    }
}