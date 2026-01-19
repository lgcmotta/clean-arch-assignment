using Asp.Versioning;
using Asp.Versioning.Builder;
using OrderManagement.Infrastructure.Persistence.Contexts;
using OrderManagement.Infrastructure.Persistence.Extensions;
using Scalar.AspNetCore;

namespace OrderManagement.WebApi.Extensions;

internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        internal WebApplication UsePermissiveCors()
        {
            app.UseCors("Permissive");

            return app;
        }

        internal WebApplication MapOpenApiUI()
        {
            app.MapScalarApiReference(options =>
            {
                options.WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .WithOpenApiRoutePattern("/openapi/{documentName}.json")
                    .WithTitle("Order Management API");
            });

            return app;
        }

        internal IEndpointRouteBuilder MapApiGroup(ApiVersion version)
        {
            var versionSet = app.BuildApiVersionSet(version);

            return app.MapGroup("/api/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);
        }

        public async ValueTask SetupSqlServerAsync()
        {
            await using var scope = app.Services.CreateAsyncScope();
            await using var context = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>();
            await context.MigrateSqlServerAsync();
            await context.SeedCustomerIfNone();
        }

        private ApiVersionSet BuildApiVersionSet(ApiVersion version)
        {
            return app.NewApiVersionSet()
                .HasApiVersion(version)
                .ReportApiVersions()
                .Build();
        }
    }
}