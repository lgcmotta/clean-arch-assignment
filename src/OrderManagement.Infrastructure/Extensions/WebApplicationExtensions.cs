using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace OrderManagement.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    private const string ReadinessEndpointPath = "/healthz/ready";
    private const string LivenessEndpointPath = "/healthz/live";

    extension(WebApplication app)
    {
        public WebApplication MapDefaultEndpoints()
        {
            if (!app.Environment.IsDevelopment())
            {
                return app;
            }

            app.MapHealthChecks(ReadinessEndpointPath, WebApplication.CreateHealthCheckOptions("ready"));

            app.MapHealthChecks(LivenessEndpointPath, WebApplication.CreateHealthCheckOptions("alive"));

            return app;
        }

        private static HealthCheckOptions CreateHealthCheckOptions(string tag)
        {
            return new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("ready"),
                ResponseWriter = (context, report) => context.HealthCheckResponseWriter(report),
                AllowCachingResponses = false
            };
        }
    }
}