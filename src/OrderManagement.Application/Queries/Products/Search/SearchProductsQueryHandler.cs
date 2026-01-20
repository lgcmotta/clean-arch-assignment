using MediatR;
using OrderManagement.Application.Models.Products;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Application.Repositories.Products;

namespace OrderManagement.Application.Queries.Products.Search;

internal sealed class SearchProductsQueryHandler(IProductReadRepository repository) : IRequestHandler<SearchProductsQuery, SearchProductsResponse>
{
    public async Task<SearchProductsResponse> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var total = await repository.CountTotalProductsAsync(cancellationToken);

        (IEnumerable<ProductReadModel> models, PagedResponseModel pagination) = await repository.PaginateProductsAsync(total, request, cancellationToken);

        return new SearchProductsResponse(models, pagination);
    }
}