using HashidsNet;
using MediatR;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Repositories;

namespace OrderManagement.Application.Commands.Products.CreateProduct;

internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductWriteRepository _repository;
    private readonly IHashids _hashids;

    public CreateProductCommandHandler(IProductWriteRepository repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsAsync(request.Name, cancellationToken))
        {
            throw new DuplicatedProductNameException(request.Name);
        }

        var product = new Product(request.Name, request.Price);

        await _repository.AddProductAsync(product, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        product.RaiseProductCreatedDomainEvent();

        return new CreateProductResponse(Id: _hashids.EncodeLong(product.Id), product.Name, product.Price);
    }
}