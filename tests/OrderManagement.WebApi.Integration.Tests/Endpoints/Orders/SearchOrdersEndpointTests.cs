namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Orders;

public sealed class SearchOrdersEndpointTests
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public SearchOrdersEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SearchOrders_WhenOrdersExist_ShouldReturnPagedOrders()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var repository = scope.ServiceProvider.GetRequiredService<IOrderReadRepository>();
        var customerId = hashids.EncodeLong(Faker.Random.Long(1, 100000));
        var orderId = hashids.EncodeLong(Faker.Random.Long(1, 100000));
        var productId = hashids.EncodeLong(Faker.Random.Long(1, 100000));
        var unitPrice = decimal.Round(Faker.Random.Decimal(1, 200), 2);
        var quantity = Faker.Random.Int(1, 5);
        var totalPrice = unitPrice * quantity;
        var model = new OrderReadModel
        {
            Id = orderId,
            CreatedDate = DateTimeOffset.UtcNow,
            TotalAmount = totalPrice,
            Status = OrderStatus.Created.Value,
            Customer = new CustomerOrderReadModel
            {
                Id = customerId,
                Name = Faker.Name.FullName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber()
            },
            Items =
            [
                new OrderItemReadModel
                {
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    Product = new ProductOrderItemReadModel
                    {
                        Id = productId,
                        Name = Faker.Commerce.ProductName(),
                        Price = unitPrice
                    }
                }
            ]
        };
        await repository.SyncOrderCreatedAsync(model, cancellationToken);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.GetFromJsonAsync<PagedApiResponse<IEnumerable<OrderReadModel>>>(
            $"/api/v1/customers/{customerId}/orders?page=1&size=10&sort=ASC",
            cancellationToken);

        // Assert
        response.ShouldNotBeNull();
        response!.Data.Any(order => order.Id == orderId).ShouldBeTrue();
        response.Pagination.Page.ShouldBe(1);
    }

    [Fact]
    public async Task SearchOrders_WhenPaginationIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var customerId = hashids.EncodeLong(12345);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/v1/customers/{customerId}/orders?page=0&size=0&sort=ASC", cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}