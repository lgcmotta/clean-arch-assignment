using OrderManagement.Application.Shared;

namespace OrderManagement.WebApi.Responses;

public record ApiResponse<TResponse>(TResponse Data);

public record PagedApiResponse<TResponse>(TResponse Data, PagedResponse Pagination) : ApiResponse<TResponse>(Data);