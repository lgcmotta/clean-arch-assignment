using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Aggregates.Customers;
using OrderManagement.Domain.Core;

namespace OrderManagement.Infrastructure.Persistence.Extensions;

public static class DbContextExtensions
{
    extension<TDbContext>(TDbContext context) where TDbContext : DbContext
    {
        public async ValueTask MigrateSqlServerAsync()
        {
            await context.Database.MigrateAsync();
        }

        public async ValueTask SeedCustomerIfNone()
        {
            var customer = await context.Set<Customer>().FirstOrDefaultAsync(c => c.Id == 1);

            if (customer is not null)
            {
                return;
            }

            customer = new Customer("John Doe", "john.doe@gmail.com", "+5551999999999");

            await context.Set<Customer>().AddAsync(customer);

            await context.SaveChangesAsync();
        }


        public IDomainEvent[] ExtractAndClearDomainEvents()
        {
            var entries = context.ChangeTracker.Entries<IAggregateRoot>().Select(entry => entry).ToArray();

            var events = entries.SelectMany(entry => entry.Entity.Events).ToArray();

            foreach (var entry in entries)
            {
                entry.Entity.ClearDomainEvents();
            }

            return events;
        }
    }
}