using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Core;

namespace OrderManagement.Infrastructure.Persistence.Mappings.Core;

public class AggregateEntityTypeConfiguration<TAggregate> : IEntityTypeConfiguration<TAggregate>
    where TAggregate : class, IAggregateRoot
{
    public void Configure(EntityTypeBuilder<TAggregate> builder)
    {
        builder.Ignore(aggregate => aggregate.Events);
    }
}