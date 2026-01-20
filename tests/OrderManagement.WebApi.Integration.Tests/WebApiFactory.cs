[assembly: AssemblyFixture(typeof(WebApiFactory))]

namespace OrderManagement.WebApi.Integration.Tests;

[UsedImplicitly]
public sealed class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sql = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
        .WithPortBinding(1433, assignRandomHostPort: true)
        .Build();

    private readonly MongoDbContainer _mongo = new MongoDbBuilder("mirror.gcr.io/mongo:8")
        .WithPortBinding(27017, assignRandomHostPort: true)
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _mongo.StartAsync(TestContext.Current.CancellationToken);
        await _sql.StartAsync(TestContext.Current.CancellationToken);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:SqlServer", _sql.GetConnectionString());
        builder.UseSetting("ConnectionStrings:MongoDB", _mongo.GetConnectionString());

        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:SqlServer", _sql.GetConnectionString()),
                new KeyValuePair<string, string?>("ConnectionStrings:MongoDB", _mongo.GetConnectionString())
            ]);
        });
    }

    public HttpClient CreateHttpClient()
    {
        var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = Server.BaseAddress };

        return CreateClient(options);
    }
}