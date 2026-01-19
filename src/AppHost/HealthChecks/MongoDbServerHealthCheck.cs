using AppHost.Resources;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace AppHost.HealthChecks;

public sealed class MongoDbServerHealthCheck(MongoDbReplicaSetResource resource) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var connectionString = await resource.ConnectionStringExpression.GetValueAsync(cancellationToken);

        var settings = MongoClientSettings.FromConnectionString(connectionString);

        settings.DirectConnection = true;
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);

        using var client = new MongoClient(settings);

        var admin = client.GetDatabase("admin");

        try
        {
            await admin.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("MongoDB ping failed", exception);
        }
    }
}