namespace OrderManagement.Domain.Tests.Customers;

[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public sealed class CustomerTests
{
    private static readonly Faker Faker = new();

    public static TheoryData<string?> InvalidValues =>
    [
        null!,
        string.Empty,
        "   "
    ];

    [Fact]
    public void CreateCustomer_WhenDetailsAreValid_ShouldSetProperties()
    {
        // Arrange
        var name = Faker.Name.FullName();
        var email = Faker.Internet.Email();
        var phone = Faker.Phone.PhoneNumber();

        // Act
        var customer = new Customer(name, email, phone);

        // Assert
        customer.Name.ShouldBe(name);
        customer.Email.ShouldBe(email);
        customer.Phone.ShouldBe(phone);
    }

    [Theory]
    [MemberData(nameof(InvalidValues))]
    public void CreateCustomer_WhenNameIsInvalid_ShouldThrowArgumentException(string? name)
    {
        // Arrange
        var email = Faker.Internet.Email();
        var phone = Faker.Phone.PhoneNumber();

        // Act
        var act = () => new Customer(name!, email, phone);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    [Theory]
    [MemberData(nameof(InvalidValues))]
    public void CreateCustomer_WhenEmailIsInvalid_ShouldThrowArgumentException(string? email)
    {
        // Arrange
        var name = Faker.Name.FullName();
        var phone = Faker.Phone.PhoneNumber();

        // Act
        var action = () => new Customer(name, email!, phone);

        // Assert
        Should.Throw<ArgumentException>(action);
    }

    [Theory]
    [MemberData(nameof(InvalidValues))]
    public void CreateCustomer_WhenPhoneIsInvalid_ShouldThrowArgumentException(string? phone)
    {
        // Arrange
        var name = Faker.Name.FullName();
        var email = Faker.Internet.Email();

        // Act
        var action = () => new Customer(name, email, phone!);

        // Assert
        Should.Throw<ArgumentException>(action);
    }

    [Fact]
    public void RaiseCustomerCreatedDomainEvent_WhenCalled_ShouldAddDomainEvent()
    {
        // Arrange
        var customer = new Customer(Faker.Name.FullName(), Faker.Internet.Email(), Faker.Phone.PhoneNumber());

        // Act
        customer.RaiseCustomerCreatedDomainEvent();

        // Assert
        customer.Events.Single().ShouldBeOfType<CustomerCreatedDomainEvent>();
    }
}