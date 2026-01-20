namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Orders;

public sealed class CreateOrderEndpointTests
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public CreateOrderEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateOrder_WhenRequestIsValid_ShouldReturnCreatedOrder()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var customer = new Customer(Faker.Name.FullName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber());
        var product = new Product(Faker.Commerce.ProductName(), Faker.Finance.Amount());
        await context.Set<Customer>().AddAsync(customer, cancellationToken);
        await context.Set<Product>().AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        var customerId = hashids.EncodeLong(customer.Id);
        var productId = hashids.EncodeLong(product.Id);
        var request = new CreateOrderCommand(customerId, [new CreateOrderItem(productId, Faker.Random.Int(1, 5))]);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/customers/{customerId}/orders", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<CreateOrderResponse>>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        payload.ShouldNotBeNull();
        payload!.Data.Id.ShouldNotBeNullOrWhiteSpace();
        payload.Data.CustomerId.ShouldBe(customerId);
        payload.Data.Status.ShouldBe(OrderStatus.Created.Value);
    }

    [Fact]
    public async Task CreateOrder_WhenBodyIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new CreateOrderCommand(string.Empty, Array.Empty<CreateOrderItem>());
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/customers/invalid/orders", request, cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}