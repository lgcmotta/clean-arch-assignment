namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Products;

public sealed class SearchProductsEndpointTests
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public SearchProductsEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SearchProducts_WhenProductsExist_ShouldReturnPagedProducts()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var repository = scope.ServiceProvider.GetRequiredService<IProductReadRepository>();
        var productId = hashids.EncodeLong(Random.Shared.NextInt64(1, 250));
        var model = new ProductReadModel
        {
            Id = productId,
            Name = Faker.Commerce.ProductName(),
            Price = Faker.Finance.Amount()
        };
        await repository.SyncReadModelAsync(model, cancellationToken);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.GetFromJsonAsync<PagedApiResponse<IEnumerable<ProductReadModel>>>(
            "/api/v1/products?page=1&size=10&sort=ASC",
            cancellationToken);

        // Assert
        response.ShouldNotBeNull();
        response!.Data.Any(product => product.Id == productId).ShouldBeTrue();
        response.Pagination.Page.ShouldBe(1);
    }

    [Fact]
    public async Task SearchProducts_WhenPaginationIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.GetAsync("/api/v1/products?page=0&size=0&sort=ASC", cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}