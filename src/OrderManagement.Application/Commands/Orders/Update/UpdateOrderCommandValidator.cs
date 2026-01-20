using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Orders.Update;

[UsedImplicitly]
public sealed class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty();
        RuleFor(command => command.OrderId).NotEmpty();

        When(command => command.Items is not null, () =>
        {
            RuleForEach(command => command.Items!)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId).NotEmpty();
                    item.RuleFor(i => i.Quantity).GreaterThan(0);
                });
        });
    }
}