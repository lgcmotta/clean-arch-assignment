using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Models.Products;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Products;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Queries.Products.Search;

[UsedImplicitly]
public record SearchProductsQuery : PagedQueryModel, IRequest<SearchProductsResponse>, IQuery;

public record SearchProductsResponse(IEnumerable<ProductReadModel> Products, PagedResponseModel Pagination);

public sealed class SearchProductsQueryHandler(IProductReadRepository repository) : IRequestHandler<SearchProductsQuery, SearchProductsResponse>
{
    public async Task<SearchProductsResponse> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var total = await repository.CountTotalProductsAsync(cancellationToken);

        (IEnumerable<ProductReadModel> models, PagedResponseModel pagination) = await repository.PaginateProductsAsync(total, request, cancellationToken);

        return new SearchProductsResponse(models, pagination);
    }
}