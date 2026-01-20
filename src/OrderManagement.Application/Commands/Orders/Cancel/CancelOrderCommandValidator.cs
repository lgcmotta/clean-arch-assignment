using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Orders.Cancel;

[UsedImplicitly]
internal sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty()
            .WithName("customerId")
            .WithMessage("'customerId' must not be null, empty or white-space.");

        RuleFor(command => command.OrderId)
            .NotEmpty()
            .WithName("orderId")
            .WithMessage("'orderId' must not be null, empty or white-space.");
    }
}