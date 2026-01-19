using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Shared;

namespace OrderManagement.WebApi.Requests;

[UsedImplicitly]
public record PaginationRequest(
    [property: FromQuery] int Page = 1,
    [property: FromQuery] int Size = 50,
    [property: FromQuery] SortingOrder Sort = SortingOrder.ASC,
    [property: FromQuery] string? SortBy = null);