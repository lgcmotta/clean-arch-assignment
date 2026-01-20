using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Aggregates.Customers;
using OrderManagement.Domain.Aggregates.Orders;

namespace OrderManagement.Infrastructure.Persistence.Mappings.Customers;

public sealed class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "customers");

        builder.Property(customer => customer.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(customer => customer.Phone)
            .HasMaxLength(25)
            .IsRequired();

        builder.HasMany<Order>()
            .WithOne()
            .HasForeignKey(order => order.CustomerId);
    }
}