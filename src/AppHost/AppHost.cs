using AppHost.Extensions;
using Projects;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo", 27017)
    .WithImageRegistry("mirror.gcr.io")
    .WithImage("mongo:8")
    .WithDataVolume("mongodb_data")
    .WithReplicaSet("./Keys/keyfile")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("read")
    .AddReplicaSet("MongoDB");

var sqlserver = builder.AddSqlServer("sql-server", port: 1433)
    .WithImageRegistry("mcr.microsoft.com/mssql")
    .WithImage("server:2025-latest")
    .WithDataVolume("sqlserver_data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("SqlServer", "write");

var webapi = builder.AddProject<OrderManagement_WebApi>("orders-api")
    .WithReference(mongo)
    .WithReference(sqlserver)
    .WaitFor(mongo)
    .WaitFor(sqlserver)
    .WithHttpHealthCheck("/healthz/ready")
    .WithHttpHealthCheck("/healthz/live")
    .WithEnvironment("HashIds__Salt", "209a18feadf2410f8c3fa15dddf49ea5")
    .WithEnvironment("HashIds__MinHashLength", "7");

var scalar = builder.AddScalarApiReference();

scalar.WithApiReference(webapi, options =>
    {
        options.AddDocument("v1", "Order Management API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithOpenApiRoutePattern("/openapi/{documentName}.json");
    })
    .WaitFor(webapi);

var app = builder.Build();

await app.RunAsync();