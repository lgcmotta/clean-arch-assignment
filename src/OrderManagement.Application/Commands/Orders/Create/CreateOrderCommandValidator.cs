using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Orders.Create;

[UsedImplicitly]
internal sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty()
            .WithName("customerId")
            .WithMessage("'customerId' must not be null, empty or white-space.");

        RuleFor(command => command.Items)
            .NotEmpty()
            .WithName("items")
            .WithMessage("At least one item must be provided.");

        RuleForEach(command => command.Items)
            .ChildRules(validator =>
            {
                validator.RuleFor(item => item.ProductId)
                    .NotEmpty()
                    .WithName("productId")
                    .WithMessage("'productId' must not be null, empty or white-space.");

                validator.RuleFor(item => item.Quantity)
                    .GreaterThan(0)
                    .WithName("quantity")
                    .WithMessage("'quantity' must be greater than zero.");
            });
    }
}
