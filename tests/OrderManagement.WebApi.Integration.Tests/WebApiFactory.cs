using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace OrderManagement.WebApi.Integration.Tests;

public class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sql = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
        .WithAcceptLicenseAgreement(true)
        .WithPortBinding(1433, assignRandomHostPort: true)
        .Build();

    private readonly MongoDbContainer _mongo = new MongoDbBuilder("mirror.gcr.io/mongo:8")
        .WithAcceptLicenseAgreement(true)
        .WithPortBinding(27017, assignRandomHostPort: true)
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _mongo.StartAsync(TestContext.Current.CancellationToken);
        await _sql.StartAsync(TestContext.Current.CancellationToken);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings__SqlServer", _sql.GetConnectionString());
        builder.UseSetting("ConnectionStrings__MongoDB", _mongo.GetConnectionString());
    }

    public HttpClient CreateHttpClient()
    {
        var options = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = Server.BaseAddress
        };

        return CreateClient(options);
    }
}