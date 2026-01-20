namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Products;

public sealed class CreateProductEndpointTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public CreateProductEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateProduct_WhenBodyIsCorrect_ShouldReturnCreatedProduct()
    {
        // Arrange

        // Act

        // Assert
    }
}