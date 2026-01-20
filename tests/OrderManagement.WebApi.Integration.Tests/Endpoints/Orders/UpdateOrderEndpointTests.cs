namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Orders;

public sealed class UpdateOrderEndpointTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public UpdateOrderEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UpdateOrder_WhenRequestIsValid_ShouldReturnUpdatedOrder()
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
        var request = new UpdateOrderCommand(customerId, orderId, OrderStatus.PendingPayment.Value);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.PatchAsJsonAsync($"/api/v1/customers/{customerId}/orders/{orderId}", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<UpdateOrderResponse>>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        payload.ShouldNotBeNull();
        payload!.Data.Id.ShouldBe(orderId);
        payload.Data.Status.ShouldBe(OrderStatus.PendingPayment.Value);
    }

    [Fact]
    public async Task UpdateOrder_WhenBodyIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new UpdateOrderCommand(string.Empty, string.Empty, OrderStatus.PendingPayment.Value);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.PatchAsJsonAsync("/api/v1/customers/invalid/orders/invalid", request, cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}
