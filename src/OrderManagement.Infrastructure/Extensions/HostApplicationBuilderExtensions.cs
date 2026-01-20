using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OrderManagement.Infrastructure.HealthChecks;

namespace OrderManagement.Infrastructure.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddServiceDefaults()
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

            return builder;
        }

        private TBuilder ConfigureOpenTelemetry()
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddEventCountersInstrumentation(options => options
                            .AddEventSources(
                                "Microsoft.AspNetCore.Hosting",
                                "Microsoft.AspNetCore.Http.Connections",
                                "Microsoft-AspNetCore-Server-Kestrel",
                                "System.Net.Http",
                                "System.Net.NameResolution",
                                "System.Net.Security"
                            )
                        );
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation(options => options.Filter = context => context.IsNotHealthCheck())
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private TBuilder AddOpenTelemetryExporters()
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            return builder;
        }

        private TBuilder AddDefaultHealthChecks()
        {
            builder.Services.AddHostedService<LivenessBackgroundService>();
            builder.Services.AddSingleton<LivenessHealthCheck>();

            string? connectionString = builder.Configuration.GetConnectionString("SqlServer");

            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            builder.Services.AddHealthChecks()
                .AddCheck<LivenessHealthCheck>("liveness", tags: ["alive"])
                .AddMongoDb(tags: ["ready"])
                .AddSqlServer(connectionString, tags: ["ready"]);

            return builder;
        }
    }
}