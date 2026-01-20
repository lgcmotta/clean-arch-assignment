using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Queries.Orders.Search;

[UsedImplicitly]
internal sealed class SearchOrdersQueryValidator : AbstractValidator<SearchOrdersQuery>
{
    public SearchOrdersQueryValidator()
    {
        RuleFor(query => query.CustomerId)
            .NotEmpty()
            .WithName("customerId")
            .WithMessage("'customerId' must not be null, empty or white-space.");

        RuleFor(query => query.Page)
            .GreaterThan(0)
            .WithName("page")
            .WithMessage("'page' must be greater than zero.");

        RuleFor(query => query.Size)
            .GreaterThan(0)
            .WithName("size")
            .WithMessage("'size' must be greater than zero.");
    }
}
