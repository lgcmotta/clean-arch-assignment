using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Models.Products;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Queries.Products.Search;

[UsedImplicitly]
public record SearchProductsQuery : PagedQueryModel, IRequest<SearchProductsResponse>, IQuery;

public record SearchProductsResponse(IEnumerable<ProductReadModel> Products, PagedResponseModel Pagination);