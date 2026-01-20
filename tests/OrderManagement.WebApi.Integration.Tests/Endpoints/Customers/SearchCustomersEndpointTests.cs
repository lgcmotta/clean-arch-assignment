namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Customers;

public sealed class SearchCustomersEndpointTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public SearchCustomersEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SearchCustomers_WhenCustomersExist_ShouldReturnPagedCustomers()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _factory.Services.CreateAsyncScope();
        var hashids = scope.ServiceProvider.GetRequiredService<IHashids>();
        var repository = scope.ServiceProvider.GetRequiredService<ICustomerReadRepository>();
        var customerId = hashids.EncodeLong(Faker.Random.Long(1, 100000));
        var model = new CustomerReadModel
        {
            Id = customerId,
            Name = Faker.Name.FullName(),
            Email = Faker.Internet.Email(),
            Phone = Faker.Phone.PhoneNumber()
        };
        await repository.SyncReadModelAsync(model, cancellationToken);
        var client = _factory.CreateHttpClient();

        // Act
        var response = await client.GetFromJsonAsync<PagedApiResponse<IEnumerable<CustomerReadModel>>>(
            "/api/v1/customers?page=1&size=10",
            cancellationToken);

        // Assert
        response.ShouldNotBeNull();
        response!.Data.Any(customer => customer.Id == customerId).ShouldBeTrue();
        response.Pagination.Page.ShouldBe(1);
    }

    [Fact]
    public async Task SearchCustomers_WhenPaginationIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.GetAsync("/api/v1/customers?page=0&size=0", cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}
