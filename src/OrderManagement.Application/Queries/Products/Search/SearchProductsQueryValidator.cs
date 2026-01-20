using FluentValidation;
using JetBrains.Annotations;

namespace OrderManagement.Application.Queries.Products.Search;

[UsedImplicitly]
internal sealed class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
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
