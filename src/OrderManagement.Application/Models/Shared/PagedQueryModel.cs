namespace OrderManagement.Application.Models.Shared;

public record PagedQueryModel
{
    public int Page { get; init; }
    public int Size { get; init; }
    public SortingOrder Sort { get; init; }
    public string? SortBy { get; init; }
}