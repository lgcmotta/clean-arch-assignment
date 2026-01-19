using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Aggregates.Orders.Entities;
using OrderManagement.Domain.Core;

namespace OrderManagement.Infrastructure.Persistence.Mappings;

public sealed class OrderItemSqlServerEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", "orders");

        builder.Property<int>("Id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasKey("Id");

        builder.Property(item => item.ProductId)
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(item => item.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(product => product.UnitPrice)
            .HasColumnType("bigint")
            .HasConversion(
                price => price.ToCents(),
                cents => Money.FromCents(cents)
            )
            .IsRequired();

        builder.Property(product => product.TotalPrice)
            .HasColumnType("bigint")
            .HasConversion(
                price => price.ToCents(),
                cents => Money.FromCents(cents)
            )
            .IsRequired();
    }
}