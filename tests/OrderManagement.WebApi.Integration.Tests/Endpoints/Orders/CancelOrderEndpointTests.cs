namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Orders;

public sealed class CancelOrderEndpointTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public CancelOrderEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CancelOrder_WhenOrderExists_ShouldReturnCancelledOrder()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var customer = new Customer(Faker.Name.FullName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber());
        await context.Set<Customer>().AddAsync(customer, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        var order = new Order(customer.Id, DateTimeOffset.UtcNow);
        await context.Set<Order>().AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        var customerId = hashids.EncodeLong(customer.Id);
        var orderId = hashids.EncodeLong(order.Id);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/customers/{customerId}/orders/{orderId}", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<CancelOrderResponse>>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        payload.ShouldNotBeNull();
        payload!.Data.Status.ShouldBe(OrderStatus.Cancelled.Value);
    }

    [Fact]
    public async Task CancelOrder_WhenOrderDoesNotExist_ShouldReturnProblemDetails()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var customerId = hashids.EncodeLong(9999998);
        var orderId = hashids.EncodeLong(9999999);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/customers/{customerId}/orders/{orderId}", cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.NotFound);
    }
}