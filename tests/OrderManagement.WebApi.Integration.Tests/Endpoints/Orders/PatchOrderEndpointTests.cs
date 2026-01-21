using OrderManagement.Application.Commands.Orders.Patch;

namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Orders;

public sealed class PatchOrderEndpointTests
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public PatchOrderEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PatchOrder_WhenRequestIsValid_ShouldReturnUpdatedOrder()
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
        var request = new PatchOrderCommand(customerId, orderId, OrderStatus.PendingPayment.Value);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.PatchAsJsonAsync($"/api/v1/customers/{customerId}/orders/{orderId}", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<PatchOrderResponse>>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        payload.ShouldNotBeNull();
        payload!.Data.Id.ShouldBe(orderId);
        payload.Data.Status.ShouldBe(OrderStatus.PendingPayment.Value);
    }

    [Fact]
    public async Task PatchOrder_WhenBodyIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new PatchOrderCommand(string.Empty, string.Empty, OrderStatus.PendingPayment.Value);
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