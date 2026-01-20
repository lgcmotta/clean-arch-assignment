namespace OrderManagement.Domain.Tests.Products;

[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public sealed class ProductTests
{
    private static readonly Faker Faker = new();

    public static TheoryData<string?> InvalidNames =>
    [
        null!,
        string.Empty,
        "   "
    ];

    [Fact]
    public void CreateProduct_WhenNameAndPriceAreValid_ShouldSetProperties()
    {
        // Arrange
        var name = Faker.Commerce.ProductName();
        var price = decimal.Round(Faker.Random.Decimal(1, 1000), 2);

        // Act
        var product = new Product(name, price);

        // Assert
        product.Name.ShouldBe(name);
        product.Price.Value.ShouldBe(price);
    }

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public void CreateProduct_WhenNameIsInvalid_ShouldThrowArgumentException(string? name)
    {
        // Arrange
        var price = decimal.Round(Faker.Random.Decimal(1, 1000), 2);

        // Act
        Func<Product> act = () => new Product(name!, price);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void RaiseProductCreatedDomainEvent_WhenCalled_ShouldAddDomainEvent()
    {
        // Arrange
        var product = new Product(Faker.Commerce.ProductName(), decimal.Round(Faker.Random.Decimal(1, 1000), 2));

        // Act
        product.RaiseProductCreatedDomainEvent();

        // Assert
        product.Events.Single().ShouldBeOfType<ProductCreatedDomainEvent>();
    }
}