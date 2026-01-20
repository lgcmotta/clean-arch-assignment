using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Commands.Customers.Create;

[UsedImplicitly]
internal sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithName("name")
            .WithMessage("'name' must not be null, empty or white-space.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithName("email")
            .WithMessage("'email' must not be null, empty or white-space.");

        RuleFor(command => command.Phone)
            .NotEmpty()
            .WithName("phone")
            .WithMessage("'phone' must not be null, empty or white-space.");
    }
}