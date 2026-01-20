using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;

namespace OrderManagement.Infrastructure.Extensions;

internal static class HttpContextExtensions
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower };

    extension(HttpContext context)
    {
        internal bool IsNotHealthCheck()
        {
            return !context.Request.Path.StartsWithSegments("/healthz/ready")
                   && !context.Request.Path.StartsWithSegments("/healthz/live");
        }

        internal async Task HealthCheckResponseWriter(HealthReport report)
        {
            (int statusCode, string contentType) = report.Status switch
            {
                HealthStatus.Unhealthy => (StatusCodes.Status503ServiceUnavailable, MediaTypeNames.Application.ProblemJson),
                HealthStatus.Degraded => (StatusCodes.Status200OK, MediaTypeNames.Application.ProblemJson),
                _ => (StatusCodes.Status200OK, MediaTypeNames.Application.Json),
            };

            var response = new
            {
                Status = report.Status.ToString(),
                TotalDuration = report.TotalDuration.ToString("c"),
                Entries = report.Entries.ToDictionary(entry => JsonNamingPolicy.SnakeCaseLower.ConvertName(entry.Key))
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType;

            await context.Response.WriteAsJsonAsync(response, Options);
        }
    }
}