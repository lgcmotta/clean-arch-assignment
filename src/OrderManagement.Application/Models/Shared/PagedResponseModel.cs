namespace OrderManagement.Application.Models.Shared;

public record PagedResponseModel
{
    public int Page { get; init; }

    public int Size { get; init; }

    public int Previous { get; init; }

    public int Next { get; init; }

    public long Total { get; init; }

    public int TotalPages { get; init; }
}