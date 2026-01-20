using OrderManagement.Application.Models.Shared;

namespace OrderManagement.Infrastructure.Persistence.Extensions;

internal static class PagedQueryExtensions
{
    extension(PagedQueryModel query)
    {
        internal PagedResponseModel ToPagedResponse(long total)
        {
            var page = Math.Max(1, query.Page);

            var size = Math.Max(1, query.Size);

            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

            var currentPage = Math.Min(page, totalPages);

            var previous = currentPage > 1 ? currentPage - 1 : 1;

            var next = currentPage < totalPages ? currentPage + 1 : totalPages;

            return new PagedResponseModel
            {
                Page = page,
                Size = size,
                TotalPages = totalPages,
                Previous = previous,
                Next = next,
                Total = total
            };
        }
    }
}