using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Aggregates.Customers;
using OrderManagement.Domain.Aggregates.Customers.Repositories;
using OrderManagement.Domain.Aggregates.Orders;
using OrderManagement.Infrastructure.Persistence.Contexts;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Customers;

public class CustomerWriteRepository(AppDbContext context) : ICustomerWriteRepository
{
    public async ValueTask<Customer?> FindCustomerByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.Set<Customer>().AsNoTracking().SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public async ValueTask<(Customer?, Order?)> FindCustomerWithOrderAsync(long customerId, long orderId, CancellationToken cancellationToken = default)
    {
        var response = await context.Set<Customer>().AsNoTracking()
            .Where(customer => customer.Id == customerId)
            .LeftJoin(
                context.Set<Order>().Where(order => order.Id == orderId),
                customer => customer.Id,
                order => order.CustomerId,
                (customer, order) => new { Customer = customer, Order = order }
            )
            .SingleOrDefaultAsync(cancellationToken);

        return (response?.Customer, response?.Order);
    }
}