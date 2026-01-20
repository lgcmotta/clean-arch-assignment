namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Products;

public sealed class CreateProductEndpointTests
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public CreateProductEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateProduct_WhenBodyIsValid_ShouldReturnCreatedProduct()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new CreateProductCommand(Faker.Commerce.ProductName(), decimal.Round(Faker.Random.Decimal(1, 1000), 2));
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<CreateProductResponse>>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        payload.ShouldNotBeNull();
        payload!.Data.Id.ShouldNotBeNullOrWhiteSpace();
        payload.Data.Name.ShouldBe(request.Name);
        payload.Data.Price.ShouldBe(request.Price);
    }

    [Fact]
    public async Task CreateProduct_WhenBodyIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new CreateProductCommand(string.Empty, decimal.Zero);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products", request, cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}