using OrderManagement.Application.Models;
using OrderManagement.Application.Models.Products;
using OrderManagement.Application.Models.Shared;

namespace OrderManagement.Application.Repositories.Products;

public interface IProductReadRepository
{
    public ValueTask<(IEnumerable<ProductReadModel>, PagedResponseModel)> PaginateProductsAsync(long total,
        PagedQueryModel query,
        CancellationToken cancellationToken = default);

    public ValueTask<long> CountTotalProductsAsync(CancellationToken cancellationToken = default);

    public ValueTask SyncReadModelAsync(ProductReadModel model, CancellationToken cancellationToken = default);
}