using Asp.Versioning;
using OrderManagement.WebApi.Endpoints;
using OrderManagement.WebApi.Extensions;

var v1 = new ApiVersion(majorVersion: 1, minorVersion: 0);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDbClient(builder.Configuration);
builder.Services.AddSqlServerDbContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddCQRS();
builder.Services.AddIdHasher(builder.Configuration);
builder.Services.AddDomainEventsPubSub();
builder.Services.AddApiVersioning(v1);
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy("Permissive", cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

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

if (app.Environment.IsDevelopment())
{
    await app.SetupSqlServerAsync();
}

await app.RunAsync();