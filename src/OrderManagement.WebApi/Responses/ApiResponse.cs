using OrderManagement.Application.Models;
using OrderManagement.Application.Models.Shared;

namespace OrderManagement.WebApi.Responses;

public record ApiResponse<TResponse>(TResponse Data);

public record PagedApiResponse<TResponse>(TResponse Data, PagedResponseModel Pagination) : ApiResponse<TResponse>(Data);