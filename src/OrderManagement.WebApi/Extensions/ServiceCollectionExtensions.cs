using Asp.Versioning;
using FluentValidation;
using HashidsNet;
using MediatR.NotificationPublishers;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OrderManagement.Application.Behaviors;
using OrderManagement.Application.Options;
using OrderManagement.Application.Shared;
using OrderManagement.Domain.Aggregates.Customers.Repositories;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;
using OrderManagement.Infrastructure.Persistence.Behaviors;
using OrderManagement.Infrastructure.Persistence.Consumers;
using OrderManagement.Infrastructure.Persistence.Contexts;
using OrderManagement.Infrastructure.Persistence.Interceptors;
using OrderManagement.Infrastructure.Persistence.Models;
using OrderManagement.Infrastructure.Persistence.Repositories;
using System.Threading.Channels;

namespace OrderManagement.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
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

            return services.AddSingleton<IHashids>(_ => new Hashids(options.Salt, options.MinHashLength));
        }

        public IServiceCollection AddCQRS()
        {
            services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);

            return services.AddMediatR(options =>
            {
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
                options.AddOpenBehavior(typeof(DomainEventPublisherBehavior<,>));
                options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
                options.NotificationPublisherType = typeof(TaskWhenAllPublisher);
            });
        }

        public IServiceCollection AddMongoDbClient(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDBReadOnly");

            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.AddScoped<IMongoClient>(_ => new MongoClient(connectionString));

            return services;
        }

        public IServiceCollection AddSqlServerDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServer");

            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.AddDbContext<SqlServerDbContext>((provider, options) =>
            {
                options.UseSqlServer(connectionString, (sql) =>
                {
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "maintenance");
                    sql.EnableRetryOnFailure(maxRetryCount: 5);
                });

                options.AddInterceptors(InterceptorAssemblyScanner.Scan(provider, typeof(SqlServerDbContext).Assembly));
            });

            return services;
        }

        public IServiceCollection AddRepositories()
        {
            return services
                .AddScoped<IProductWriteRepository, ProductWriteRepository>()
                .AddScoped<IProductSyncRepository, ProductSyncRepository>()
                .AddScoped<IProductReadRepository<ProductDocumentModel>, ProductReadRepository>()
                .AddScoped<ICustomerWriteRepository, CustomerWriteRepository>()
                .AddScoped<ICustomerReadRepository, CustomerReadRepository>()
                .AddScoped<IOrderWriteRepository, OrderWriteRepository>()
                .AddScoped<IOrderReadRepository, OrderReadRepository>();
        }
    }
}