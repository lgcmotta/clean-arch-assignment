using JetBrains.Annotations;
using MediatR;
using OrderManagement.Application.Models.Customer;
using OrderManagement.Application.Models.Shared;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Queries.Customers.Search;

[UsedImplicitly]
public record SearchCustomersQuery : PagedQueryModel, IRequest<SearchCustomersResponse>, IQuery;

public record SearchCustomersResponse(IEnumerable<CustomerReadModel> Customers, PagedResponseModel Pagination);