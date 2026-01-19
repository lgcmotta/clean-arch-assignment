using AppHost.Resources;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AppHost.HealthChecks;

internal static class HealthCheckRegistrationFactory
{
    internal static HealthCheckRegistration MongoDbServer(MongoDbReplicaSetResource resource)
    {
        return new HealthCheckRegistration(
            name: "mongo_check",
            factory: _ => new MongoDbServerHealthCheck(resource),
            HealthStatus.Unhealthy,
            tags: [],
            timeout: TimeSpan.FromSeconds(30)
        );
    }

    internal static HealthCheckRegistration MongoDbReplicaSet(MongoDbReplicaSetResource resource)
    {
        return new HealthCheckRegistration(
            name: "mongo_replica_check",
            factory: _ => new MongoDbReplicaSetHealthCheck(resource),
            HealthStatus.Unhealthy,
            tags: [],
            timeout: TimeSpan.FromSeconds(60)
        );
    }
}