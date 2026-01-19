namespace OrderManagement.Application.Shared;

public record PagedQuery(int Page = 1, int Size = 50, SortingOrder Sort = SortingOrder.ASC, string? SortBy = null);

public record PagedResponse(int Page, int Previous, int Next, long Total);