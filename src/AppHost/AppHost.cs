using AppHost.Extensions;
using Projects;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo-db", 27017)
    .WithImageRegistry("mirror.gcr.io")
    .WithImage("mongo:8")
    .WithDataVolume("mongodb_data")
    .WithReplicaSet("./Keys/keyfile")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("MongoDB")
    .AddReplicaSet("MongoDBReadOnly");

var sqlserver = builder.AddSqlServer("sql-server", port: 1433)
    .WithImageRegistry("mcr.microsoft.com/mssql")
    .WithImage("server:2025-latest")
    .WithDataVolume("sqlserver_data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("SqlServer", "ordering");

var webapi = builder.AddProject<OrderManagement_WebApi>("orders-api")
    .WithReference(mongo)
    .WithReference(sqlserver)
    .WaitFor(mongo)
    .WaitFor(sqlserver);

var scalar = builder.AddScalarApiReference();

scalar.WithApiReference(webapi, options =>
    {
        options.AddDocument("v1", "Order Management API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithOpenApiRoutePattern("/openapi/{documentName}.json");
    })
    .WaitFor(webapi);

var app = builder.Build();

await app.RunAsync();