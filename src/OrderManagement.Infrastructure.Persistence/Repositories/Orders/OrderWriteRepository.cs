using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Aggregates.Orders;
using OrderManagement.Domain.Aggregates.Orders.Repositories;
using OrderManagement.Infrastructure.Persistence.Contexts;

namespace OrderManagement.Infrastructure.Persistence.Repositories.Orders;

public class OrderWriteRepository(AppDbContext context) : IOrderWriteRepository
{
    public async ValueTask AddOrderAsync(Order order, CancellationToken cancellationToken = default) =>
        await context.Set<Order>().AddAsync(order, cancellationToken);

    public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);

    public async ValueTask<Order?> FindCustomerOrderAsync(long customerId, long orderId, CancellationToken cancellationToken = default) =>
        await context.Set<Order>()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.CustomerId == customerId && order.Id == orderId, cancellationToken);

    public async ValueTask<Order?> FindUntrackedCustomerOrderAsync(long customerId, long orderId, CancellationToken cancellationToken = default) =>
        await context.Set<Order>()
            .AsNoTracking()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.CustomerId == customerId && order.Id == orderId, cancellationToken);
}