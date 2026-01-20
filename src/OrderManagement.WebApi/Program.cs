using Asp.Versioning;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.WebApi.Endpoints.Customers;
using OrderManagement.WebApi.Endpoints.Orders;
using OrderManagement.WebApi.Endpoints.Products;
using OrderManagement.WebApi.Extensions;

var v1 = new ApiVersion(majorVersion: 1, minorVersion: 0);

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMongoDbClient(builder.Configuration);
builder.Services.AddSqlServerDbContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddCQRS();
builder.Services.AddIdHasher(builder.Configuration);
builder.Services.AddDomainEventsPubSub();
builder.Services.AddApiVersioning(v1);
builder.Services.AddOpenApi();
builder.Services.AddPermissiveCors();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UsePermissiveCors();
}

app.MapOpenApiUI();
app.MapOpenApi();

var api = app.MapApiGroup(v1);

api.MapPostProducts(v1);
api.MapGetProducts(v1);
api.MapPostOrders(v1);
api.MapPatchOrders(v1);
api.MapDeleteOrders(v1);
api.MapGetOrders(v1);
api.MapPostCustomer(v1);

if (app.Environment.IsDevelopment())
{
    await app.SetupSqlServerAsync();
}

await app.RunAsync();