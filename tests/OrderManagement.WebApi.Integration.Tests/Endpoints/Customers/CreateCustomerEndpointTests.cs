namespace OrderManagement.WebApi.Integration.Tests.Endpoints.Customers;

public sealed class CreateCustomerEndpointTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;
    private static readonly Faker Faker = new();

    public CreateCustomerEndpointTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCustomer_WhenBodyIsValid_ShouldReturnCreatedCustomer()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new CreateCustomerCommand(
            Faker.Name.FullName(),
            Faker.Internet.Email(),
            Faker.Phone.PhoneNumber());
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/customers", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<CreateCustomerResponse>>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        payload.ShouldNotBeNull();
        payload!.Data.Id.ShouldNotBeNullOrWhiteSpace();
        payload.Data.Name.ShouldBe(request.Name);
        payload.Data.Email.ShouldBe(request.Email);
        payload.Data.Phone.ShouldBe(request.Phone);
    }

    [Fact]
    public async Task CreateCustomer_WhenBodyIsInvalid_ShouldReturnProblemDetails()
    {
        // Arrange
        var client = _factory.CreateHttpClient();
        var request = new CreateCustomerCommand(string.Empty, string.Empty, string.Empty);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/customers", request, cancellationToken);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.ShouldNotBeNull();
        problemDetails!.Status.ShouldBe((int)HttpStatusCode.BadRequest);
    }
}
