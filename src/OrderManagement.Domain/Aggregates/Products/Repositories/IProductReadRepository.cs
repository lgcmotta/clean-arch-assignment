namespace OrderManagement.Domain.Aggregates.Products.Repositories;

public interface IProductReadRepository<TModel> where TModel : notnull
{
    public ValueTask<IEnumerable<TModel>> GetProductsAsync(int page, int size, string? sortBy, string sort, CancellationToken cancellationToken = default);

    public ValueTask<long> CountAsync(CancellationToken cancellationToken = default);
}