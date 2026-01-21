using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Orders.Patch;

[UsedImplicitly]
public sealed class PatchOrderCommandValidator : AbstractValidator<PatchOrderCommand>
{
    public PatchOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty()
            .WithName("customerId")
            .WithMessage("'customerId' must not be null, empty or white-space.");

        RuleFor(command => command.OrderId)
            .NotEmpty()
            .WithName("orderId")
            .WithMessage("'orderId' must not be null, empty or white-space.");

        When(command => command.Items is not null, () =>
        {
            RuleForEach(command => command.Items!)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId)
                        .NotEmpty()
                        .WithName("productId")
                        .WithMessage("'productId' must not be null, empty or white-space.");

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0)
                        .WithName("quantity")
                        .WithMessage("'quantity' must be greater than zero.");
                });
        });
    }
}