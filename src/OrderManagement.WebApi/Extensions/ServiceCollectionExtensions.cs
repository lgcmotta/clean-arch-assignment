using Asp.Versioning;
using FluentValidation;
using HashidsNet;
using MediatR.NotificationPublishers;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OrderManagement.Application;
using OrderManagement.Application.Behaviors;
using OrderManagement.Application.Options;
using OrderManagement.Application.Repositories.Customers;
using OrderManagement.Application.Repositories.Orders;
using OrderManagement.Application.Repositories.Products;
using OrderManagement.Domain.Aggregates.Customers.Repositories;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;
using OrderManagement.Infrastructure.Behaviors;
using OrderManagement.Infrastructure.Consumers;
using OrderManagement.Infrastructure.Persistence.Contexts;
using OrderManagement.Infrastructure.Persistence.Interceptors;
using OrderManagement.Infrastructure.Persistence.Repositories.Customers;
using OrderManagement.Infrastructure.Persistence.Repositories.Orders;
using OrderManagement.Infrastructure.Persistence.Repositories.Products;
using OrderManagement.WebApi.Diagnostics;
using OrderManagement.WebApi.Middlewares;
using System.Threading.Channels;

namespace OrderManagement.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddExceptionHandling()
        {
            services.AddTransient<GlobalExceptionHandlerMiddleware>();

            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }

        internal IServiceCollection AddPermissiveCors()
        {
            return services.AddCors(options => options.AddPolicy("Permissive", cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }

        internal IServiceCollection AddDomainEventsPubSub()
        {
            services.AddSingleton<Channel<IDomainEvent>>(_ => Channel.CreateUnbounded<IDomainEvent>(new UnboundedChannelOptions
            {
                SingleReader = true, SingleWriter = true, AllowSynchronousContinuations = false
            }));

            services.AddHostedService<SyncConsumerBackgroundService>();

            return services;
        }

        internal IServiceCollection AddApiVersioning(ApiVersion version)
        {
            services.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.DefaultApiVersion = version;
                }).AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                })
                .EnableApiVersionBinding();

            return services;
        }

        public IServiceCollection AddIdHasher(IConfiguration configuration)
        {
            var options = configuration.GetSection(HashIdsOptions.SectionName).Get<HashIdsOptions>() ??
                          HashIdsOptions.Default;

            return services.AddSingleton<IHashids>(new Hashids(options.Salt, options.MinHashLength));
        }

        public IServiceCollection AddCQRS()
        {
            services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>(includeInternalTypes: true);

            return services.AddMediatR(options =>
            {
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
                options.AddOpenBehavior(typeof(DomainEventPublisherBehavior<,>));
                options.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
                options.NotificationPublisherType = typeof(TaskWhenAllPublisher);
            });
        }

        public IServiceCollection AddMongoDbClient()
        {
            services.AddSingleton<IMongoClient>(provider =>
            {
                IConfiguration configuration = provider.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("MongoDB");

                var settings = MongoClientSettings.FromConnectionString(connectionString);

                settings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());

                return new MongoClient(settings);
            });

            return services;
        }

        public IServiceCollection AddAppDbContext()
        {
            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("SqlServer");

                ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

                options.UseSqlServer(connectionString, (sql) =>
                {
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "maintenance");
                    sql.EnableRetryOnFailure(maxRetryCount: 5);
                });

                options.AddInterceptors(InterceptorAssemblyScanner.Scan(provider, typeof(AppDbContext).Assembly));
            });

            return services;
        }

        public IServiceCollection AddRepositories()
        {
            return services
                .AddScoped<IProductWriteRepository, ProductWriteRepository>()
                .AddScoped<IProductReadRepository, ProductReadRepository>()
                .AddScoped<ICustomerWriteRepository, CustomerWriteRepository>()
                .AddScoped<ICustomerReadRepository, CustomerReadRepository>()
                .AddScoped<IOrderWriteRepository, OrderWriteRepository>()
                .AddScoped<IOrderReadRepository, OrderReadRepository>();
        }
    }
}