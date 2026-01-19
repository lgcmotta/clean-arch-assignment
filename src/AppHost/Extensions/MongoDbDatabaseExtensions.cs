using MongoDB.Bson;
using MongoDB.Driver;

namespace AppHost.Extensions;

internal static class MongoDatabaseExtensions
{
    extension(IMongoDatabase database)
    {
        internal async Task<BsonDocument?> GetReplicaSetStatus(CancellationToken cancellationToken = default)
        {
            var command = new BsonDocument("replSetGetStatus", 1);

            return await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
        }

        internal async Task InitiateReplicaSet(string replicaSetName,
            string hostAddress,
            CancellationToken cancellationToken = default)
        {
            var document = new BsonDocument
            {
                { "_id", replicaSetName },
                {
                    "members", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "_id", 0 },
                            { "host", hostAddress }
                        }
                    }
                }
            };

            var command = new BsonDocument("replSetInitiate", document);

            await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
        }

        internal async Task<BsonDocument?> TryGetReplicaSetStatus(CancellationToken cancellationToken = default)
        {
            try
            {
                return await database.GetReplicaSetStatus(cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}