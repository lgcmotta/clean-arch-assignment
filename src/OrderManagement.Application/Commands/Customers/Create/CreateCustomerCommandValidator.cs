using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Customers.Create;

[UsedImplicitly]
internal sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty();

        RuleFor(command => command.Email)
            .NotEmpty();

        RuleFor(command => command.Phone)
            .NotEmpty();
    }
}