using HashidsNet;
using OrderManagement.Application.Extensions;
using OrderManagement.Application.Repositories.Products;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.EventHandling.Products;

public record SyncProductCreatedDomainEventHandler : IDomainEventHandler<ProductCreatedDomainEvent>
{
    private readonly IProductWriteRepository _writer;
    private readonly IProductReadRepository _reader;
    private readonly IHashids _hashids;

    public SyncProductCreatedDomainEventHandler(IProductWriteRepository writer, IProductReadRepository reader, IHashids hashids)
    {
        _writer = writer;
        _reader = reader;
        _hashids = hashids;
    }

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var product = await _writer.FindProductAsync(notification.Id, cancellationToken);

        if (product is null)
        {
            var productId = _hashids.EncodeLong(notification.Id);

            throw new ProductNotFoundForSyncException(productId);
        }

        var externalId = _hashids.EncodeLong(product.Id);

        var model = product.ToReadModel(externalId);

        await _reader.SyncReadModelAsync(model, cancellationToken);
    }
}