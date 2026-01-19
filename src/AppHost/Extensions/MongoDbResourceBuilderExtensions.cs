using AppHost.HealthChecks;
using AppHost.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AppHost.Extensions;

public static class MongoDbResourceBuilderExtensions
{
    public static IResourceBuilder<MongoDBServerResource> WithReplicaSet(
        this IResourceBuilder<MongoDBServerResource> builder, string keyFilePath, string rsName = "rs0")
    {
        return builder.WithArgs("--replSet", rsName, "--bind_ip_all", "--keyFile", "/data/keyfile")
            .WithBindMount(keyFilePath, "/data/keyfile", isReadOnly: true);
    }

    public static IResourceBuilder<MongoDbReplicaSetResource> AddReplicaSet(
        this IResourceBuilder<MongoDBDatabaseResource> builder,
        string name,
        string replicaSetName = "rs0",
        string hostAddress = "localhost:27017")
    {
        var resource = new MongoDbReplicaSetResource(name, replicaSetName, hostAddress, builder.Resource);

        builder.Resource.RemoveHealthChecks(builder.ApplicationBuilder.Services);
        builder.Resource.Parent.RemoveHealthChecks(builder.ApplicationBuilder.Services);

        builder.ApplicationBuilder.Services.AddHealthChecks()
            .Add(HealthCheckRegistrationFactory.MongoDbServer(resource))
            .Add(HealthCheckRegistrationFactory.MongoDbReplicaSet(resource));

        return builder.ApplicationBuilder.AddResource(resource)
            .WithHealthCheck("mongo_check")
            .WithHealthCheck("mongo_replica_check")
            .WithConnectionStringRedirection(builder.Resource);
    }

    private static void RemoveHealthChecks(this IResource resource, IServiceCollection services)
    {
        foreach (var annotation in resource.Annotations.OfType<HealthCheckAnnotation>().ToArray())
        {
            resource.Annotations.Remove(annotation);
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                var registration = options.Registrations.FirstOrDefault(hc => hc.Name == annotation.Key);

                if (registration is null)
                {
                    return;
                }

                options.Registrations.Remove(registration);
            });
        }
    }
}