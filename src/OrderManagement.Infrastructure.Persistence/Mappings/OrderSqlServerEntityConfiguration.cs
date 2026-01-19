using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Aggregates.Orders;
using OrderManagement.Domain.Aggregates.Orders.ValueObjects;
using OrderManagement.Domain.Core;

namespace OrderManagement.Infrastructure.Persistence.Mappings;

public sealed class OrderSqlServerEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", "orders");

        builder.Property(order => order.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasKey(order => order.Id);

        builder.Property(order => order.CreatedDate)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion(status => status.Key, key => Enumeration.ParseByKey<OrderStatus>(key))
            .IsRequired();

        builder.Ignore(order => order.TotalAmount);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property<DateTimeOffset?>("UpdatedDate")
            .IsRequired(false);
    }
}