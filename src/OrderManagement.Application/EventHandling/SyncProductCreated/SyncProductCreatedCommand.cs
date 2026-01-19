using HashidsNet;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.EventHandling.SyncProductCreated;

public record SyncProductCreatedDomainEventHandler : IDomainEventHandler<ProductCreatedDomainEvent>
{
    private readonly IProductSyncRepository _repository;
    private readonly IHashids _hashids;

    public SyncProductCreatedDomainEventHandler(IProductSyncRepository repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var product = await _repository.GetProductById(notification.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundForSyncException(_hashids.Encode(notification.Id));
        }

        await _repository.AddProductAsync(product, _hashids.Encode(product.Id), cancellationToken);
    }
}