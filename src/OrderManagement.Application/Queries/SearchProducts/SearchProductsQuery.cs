using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Shared;
using OrderManagement.Domain.Aggregates.Products.Repositories;
using OrderManagement.Domain.Core;
using OrderManagement.Infrastructure.Persistence.Models;

namespace OrderManagement.Application.Queries.SearchProducts;

[UsedImplicitly]
public record SearchProductsQuery(int Page = 1, int Size = 50, SortingOrder Sort = SortingOrder.ASC, string? SortBy = null)
    : PagedQuery(Page, Size, Sort, SortBy), IRequest<SearchProductsResponse>, IQuery;

public record ProductResponse(string Id, string Name, decimal Price);

public record SearchProductsResponse(IEnumerable<ProductResponse> Products, PagedResponse Pagination);

public sealed class SearchProductsQueryHandler(IProductReadRepository<ProductDocumentModel> repository) : IRequestHandler<SearchProductsQuery, SearchProductsResponse>
{
    public async Task<SearchProductsResponse> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);

        var size = Math.Max(1, request.Size);

        var total = await repository.CountAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        var currentPage = Math.Min(page, totalPages);

        var sortBy = request.SortBy?.Trim().ToLowerInvariant();

        var sortDirection = request.Sort.ToString().ToUpperInvariant();

        var products = await repository.GetProductsAsync(currentPage, size, sortBy, sortDirection, cancellationToken);

        var models = products.Select(product => new ProductResponse(product.HashedId, product.Name, product.Price / 100m));

        var previous = currentPage > 1 ? currentPage - 1 : 1;

        var next = currentPage < totalPages ? currentPage + 1 : totalPages;

        var pagination = new PagedResponse(currentPage, previous, next, total);

        return new SearchProductsResponse(models, pagination);
    }
}