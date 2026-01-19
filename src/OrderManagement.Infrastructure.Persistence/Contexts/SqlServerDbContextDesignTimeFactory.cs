using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OrderManagement.Infrastructure.Persistence.Exceptions;
using System.CommandLine;

namespace OrderManagement.Infrastructure.Persistence.Contexts;

internal sealed class SqlServerDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        var option = new Option<string>("--connection");

        var root = new RootCommand { option };

        string? connectionString = string.Empty;

        root.SetAction(value => connectionString = value.GetValue(option) ?? Environment.GetEnvironmentVariable("ConnectionStrings__SqlServer"));

        var result = root.Parse(args);

        result.Invoke();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new SqlServerConnectionStringNotFoundException();
        }

        var builder = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsHistoryTable("__EFMigrationsHistory", "maintenance");
            });

        return new SqlServerDbContext(builder.Options);
    }
}