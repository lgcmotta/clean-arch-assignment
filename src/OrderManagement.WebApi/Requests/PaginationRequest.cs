using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Models.Shared;

namespace OrderManagement.WebApi.Requests;

[UsedImplicitly]
public record PaginationRequest
{
    [FromQuery]
    public int Page { get; init; } = 1;

    [FromQuery]
    public int Size { get; init; } = 50;

    [FromQuery]
    public SortingOrder Sort { get; init; } = SortingOrder.ASC;

    [FromQuery]
    public string? SortBy { get; init; } = string.Empty;
}