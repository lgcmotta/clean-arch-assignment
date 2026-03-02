using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Orders.RemoveItem;

[UsedImplicitly]
internal sealed class RemoveOrderItemCommandValidator : AbstractValidator<RemoveOrderItemCommand>
{
    public RemoveOrderItemCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty()
            .WithName("customerId")
            .WithMessage("'customerId' must not be null, empty or white-space.");

        RuleFor(command => command.OrderId)
            .NotEmpty()
            .WithName("orderId")
            .WithMessage("'orderId' must not be null, empty or white-space.");

        RuleFor(command => command.ProductId)
            .NotEmpty()
            .WithName("productId")
            .WithMessage("'productId' must not be null, empty or white-space.");
    }
}