using FluentValidation;
using HashidsNet;
using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Aggregates.Products;
using OrderManagement.Domain.Aggregates.Products.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Products.CreateProduct;

[UsedImplicitly]
public record CreateProductCommand(string Name, decimal Price) : IRequest<CreateProductResponse>, ICommand;

public record CreateProductResponse(string Id, string Name, decimal Price);

[UsedImplicitly]
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Product name must not be null, empty or white-space");

        RuleFor(command => command.Price)
            .GreaterThan(decimal.Zero)
            .WithMessage("Product price must be greater than zero");
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
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

        return new CreateProductResponse(Id: _hashids.Encode(product.Id), product.Name, product.Price);
    }
}