using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Products.CreateProduct;

[UsedImplicitly]
internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
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