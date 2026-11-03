using CleanArchitecture.Domain.AggregatesModels.Baskets;
using CleanArchitecture.Domain.AggregatesModels.Orders;
using CleanArchitecture.Domain.AggregatesModels.Orders.Enums;
using CleanArchitecture.Domain.AggregatesModels.Orders.Events;
using CleanArchitecture.Domain.AggregatesModels.Shared;
using CleanArchitecture.Domain.Services.Checkout;
using FluentAssertions;

namespace CleanArchitecture.Application.UnitTests.Domain;

public sealed class OrderAndCheckoutTests
{
    [Fact]
    public void Phone_WithInvalidLength_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Phone("09123");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CheckoutService_WithPopulatedBasket_ShouldCreateOrderRaiseEventAndClearBasket()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var basket = Basket.Create(userId);
        basket.AddBasketProductItem(Guid.NewGuid(), 2, 50m, "Headphones");
        basket.AddBasketProductItem(Guid.NewGuid(), 1, 100m, "Webcam");

        var userInfo = new UserInformation(
            "Trung",
            new Phone("0912345678"),
            new Address("123 Le Loi", "Ho Chi Minh City"));

        var checkoutService = new CheckoutService();

        // Act
        var order = checkoutService.Checkout(basket, userInfo);

        // Assert
        order.UserId.Should().Be(userId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.OrderItems.Should().HaveCount(2);
        order.TotalPrice.Should().Be(200m);
        order.GetDomainEvents().Should().ContainSingle(e => e is OrderCreatedDomainEvent);
        basket.BasketProductItems.Should().BeEmpty();
    }

    [Fact]
    public void Order_StatusLifecycle_ShouldTransitionProperly()
    {
        // Arrange
        var userInfo = new UserInformation(
            "Trung",
            new Phone("0912345678"),
            new Address("123 Le Loi", "Ho Chi Minh City"));
        var items = new[] { new OrderItemDto(Guid.NewGuid(), 1, 99.5m) };
        var order = Order.Create(Guid.NewGuid(), userInfo, items);

        // Act
        order.MarkAsCompleted();

        // Assert
        order.Status.Should().Be(OrderStatus.Completed);
        Action cancelAfterComplete = () => order.Cancel();
        cancelAfterComplete.Should().Throw<InvalidOperationException>();
    }
}
