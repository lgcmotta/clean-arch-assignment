using AppHost.Extensions;
using AppHost.Resources;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace AppHost.HealthChecks;

public sealed class MongoDbReplicaSetHealthCheck(MongoDbReplicaSetResource resource) : IHealthCheck
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
            await admin.GetReplicaSetStatus(cancellationToken);
        }
        catch (MongoCommandException exception) when (exception.CodeName is "NotYetInitialized" or "NoReplicationEnabled")
        {
            await admin.InitiateReplicaSet(resource.ReplicaSetName, resource.HostAddress, cancellationToken);
        }
        catch (MongoCommandException exception) when (exception.CodeName is "InvalidReplicaSetConfig" || exception.Message.Contains("not a member", StringComparison.OrdinalIgnoreCase))
        {
            var status = await admin.TryGetReplicaSetStatus(cancellationToken);

            var version = status?.GetValue("config", null)?.AsBsonDocument.GetValue("version", 1).ToInt32() + 1 ?? 1;

            var newCfg = new BsonDocument
            {
                { "_id", resource.ReplicaSetName },
                { "version", version },
                {
                    "members",
                    new BsonArray { new BsonDocument { { "_id", 0 }, { "host", resource.HostAddress } } }
                }
            };

            var cmd = new BsonDocument { { "replSetReconfig", newCfg }, { "force", true } };

            await admin.RunCommandAsync<BsonDocument>(cmd, cancellationToken: cancellationToken);
        }

        var deadline = DateTime.UtcNow + context.Registration.Timeout;

        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var hello = await admin.RunCommandAsync<BsonDocument>(new BsonDocument("hello", 1),
                    cancellationToken: cancellationToken);

                if (hello.TryGetValue("isWritablePrimary", out var p) && p.IsBoolean && p.AsBoolean ||
                    hello.TryGetValue("ismaster", out var m) && m.IsBoolean && m.AsBoolean)
                {
                    return HealthCheckResult.Healthy();
                }
            }
            catch
            {
                // ignored
            }

            await Task.Delay(500, cancellationToken);
        }

        return HealthCheckResult.Unhealthy("MongoDB replica set not primary");
    }
}