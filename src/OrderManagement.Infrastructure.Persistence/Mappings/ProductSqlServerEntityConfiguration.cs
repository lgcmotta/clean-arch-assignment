using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Core;

namespace OrderManagement.Infrastructure.Persistence.Mappings;

public sealed class ProductSqlServerEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "products");

        builder.Property(product => product.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasColumnType("bigint")
            .HasConversion(
                price => price.ToCents(),
                cents => Money.FromCents(cents)
            )
            .IsRequired();
    }
}