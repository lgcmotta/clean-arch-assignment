namespace OrderManagement.Domain.Tests.Orders;

[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public sealed class OrderTests
{
    private static readonly Faker Faker = new();

    public static TheoryData<long> InvalidCustomerIds =>
    [
        0,
        -1
    ];

    [Fact]
    public void CreateOrder_WhenCustomerIdIsValid_ShouldSetProperties()
    {
        // Arrange
        var customerId = Faker.Random.Long(1);
        var createdDate = DateTimeOffset.UtcNow;

        // Act
        var order = new Order(customerId, createdDate);

        // Assert
        order.CustomerId.ShouldBe(customerId);
        order.CreatedDate.ShouldBe(createdDate);
        order.Status.ShouldBe(OrderStatus.Created);
    }

    [Theory]
    [MemberData(nameof(InvalidCustomerIds))]
    public void CreateOrder_WhenCustomerIdIsInvalid_ShouldThrowArgumentOutOfRangeException(long customerId)
    {
        // Arrange
        var createdDate = DateTimeOffset.UtcNow;

        // Act
        var act = () => new Order(customerId, createdDate);

        // Assert
        Should.Throw<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void IncludeItem_WhenItemIsNew_ShouldAddItem()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);
        var productId = Faker.Random.Long(1);
        var productName = Faker.Commerce.ProductName();
        var quantity = Faker.Random.Int(1, 10);
        var unitPrice = decimal.Round(Faker.Random.Decimal(1, 1000), 2);

        // Act
        order.IncludeItem(productId, productName, quantity, unitPrice);

        // Assert
        var item = order.Items.Single();
        item.ProductId.ShouldBe(productId);
        item.TotalPrice.Value.ShouldBe(unitPrice * quantity);
    }

    [Fact]
    public void IncludeItem_WhenItemExists_ShouldIncreaseQuantityAndUpdateUnitPrice()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);
        var productId = Faker.Random.Long(1);
        var productName = Faker.Commerce.ProductName();
        const int initialQuantity = 2;
        const int additionalQuantity = 3;
        const decimal initialUnitPrice = 10m;
        const decimal updatedUnitPrice = 12.5m;

        order.IncludeItem(productId, productName, initialQuantity, initialUnitPrice);

        // Act
        order.IncludeItem(productId, productName, additionalQuantity, updatedUnitPrice);

        // Assert
        var item = order.Items.Single();
        item.Quantity.ShouldBe(initialQuantity + additionalQuantity);
        item.UnitPrice.Value.ShouldBe(updatedUnitPrice);
    }

    [Fact]
    public void SetItemQuantity_WhenQuantityIsZero_ShouldRemoveItem()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);
        var productId = Faker.Random.Long(1);
        var productName = Faker.Commerce.ProductName();

        order.IncludeItem(productId, productName, 1, 10m);

        // Act
        order.SetItemQuantity(productId, productName, 0, 10m);

        // Assert
        order.Items.ShouldBeEmpty();
    }

    [Fact]
    public void SetItemQuantity_WhenItemDoesNotExist_ShouldAddItemWithQuantity()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);
        var productId = Faker.Random.Long(1);
        var productName = Faker.Commerce.ProductName();
        var quantity = 4;

        // Act
        order.SetItemQuantity(productId, productName, quantity, 15m);

        // Assert
        var item = order.Items.Single();
        item.ProductId.ShouldBe(productId);
        item.Quantity.ShouldBe(quantity);
    }

    [Fact]
    public void Cancel_WhenCurrentStatusAllows_ShouldSetStatusToCancelled()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);

        // Act
        order.Cancel();

        // Assert
        order.Status.ShouldBe(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenCurrentStatusDoesNotAllow_ShouldThrowOrderStatusTransitionException()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);
        order.ChangeStatus(OrderStatus.PendingPayment.Value);
        order.ChangeStatus(OrderStatus.PaymentApproved.Value);
        order.ChangeStatus(OrderStatus.PendingShipment.Value);
        order.ChangeStatus(OrderStatus.Shipped.Value);
        order.ChangeStatus(OrderStatus.InTransit.Value);

        // Act
        var act = order.Cancel;

        // Assert
        Should.Throw<OrderStatusTransitionException>(act);
    }

    [Fact]
    public void RaiseOrderEvent_WhenCalled_ShouldAddOrderCreatedDomainEvent()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);

        // Act
        order.RaiseOrderCreatedEvent();

        // Assert
        order.Events.Single().ShouldBeOfType<OrderCreatedDomainEvent>();
    }

    [Fact]
    public void RaiseOrderEvent_WhenCalled_ShouldAddOrderUpdatedDomainEvent()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);

        // Act
        order.RaiseOrderUpdatedEvent();

        // Assert
        order.Events.Single().ShouldBeOfType<OrderUpdatedDomainEvent>();
    }

    [Fact]
    public void RaiseOrderEvent_WhenCalled_ShouldAddOrderCancelledDomainEvent()
    {
        // Arrange
        var order = new Order(Faker.Random.Long(1), DateTimeOffset.UtcNow);

        // Act
        order.RaiseOrderCancelledEvent();

        // Assert
        order.Events.Single().ShouldBeOfType<OrderCancelledDomainEvent>();
    }
}