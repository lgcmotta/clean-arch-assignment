using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Orders.Cancel;

[UsedImplicitly]
internal sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty();
        RuleFor(command => command.OrderId).NotEmpty();
    }
}