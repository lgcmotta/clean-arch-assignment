using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppHost.Resources;

public sealed class MongoDbReplicaSetResource(
    string name,
    string replicaSet,
    string hostAddress,
    MongoDBDatabaseResource parent)
    : Resource(name),
        IResourceWithConnectionString,
        IResourceWithParent<MongoDBDatabaseResource>
{
    public ReferenceExpression ConnectionStringExpression
    {
        get
        {
            var builder = new ReferenceExpressionBuilder();
            builder.AppendFormatted(Parent.ConnectionStringExpression);
            builder.AppendLiteral($"&replicaSet={ReplicaSetName}");
            return builder.Build();
        }
    }

    public MongoDBDatabaseResource Parent { get; } = parent;

    public string ReplicaSetName { get; } = ThrowIfNullOrWhiteSpace(replicaSet);

    public string HostAddress { get; } = ThrowIfNullOrWhiteSpace(hostAddress);

    private static string ThrowIfNullOrWhiteSpace(
        [NotNull] string? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
        return argument;
    }
}