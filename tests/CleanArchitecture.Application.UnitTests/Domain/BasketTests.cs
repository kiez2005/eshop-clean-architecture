using CleanArchitecture.Domain.AggregatesModels.Baskets;
using FluentAssertions;

namespace CleanArchitecture.Application.UnitTests.Domain;

public sealed class BasketTests
{
    [Fact]
    public void AddBasketProductItem_NewProduct_ShouldAddItemAndCalculateTotalPrice()
    {
        // Arrange
        var basket = Basket.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();

        // Act
        basket.AddBasketProductItem(productId, 2, 15.5m, "USB-C Hub");

        // Assert
        basket.BasketProductItems.Should().HaveCount(1);
        basket.TotalPrice.Should().Be(31.0m);
    }

    [Fact]
    public void AddBasketProductItem_ExistingProduct_ShouldMergeQuantity()
    {
        // Arrange
        var basket = Basket.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();
        basket.AddBasketProductItem(productId, 1, 20m, "Laptop Stand");

        // Act
        basket.AddBasketProductItem(productId, 3, 20m, "Laptop Stand");

        // Assert
        basket.BasketProductItems.Should().HaveCount(1);
        basket.BasketProductItems.First().Quantity.Should().Be(4);
        basket.TotalPrice.Should().Be(80m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void AddBasketProductItem_WithNonPositiveQuantity_ShouldThrowArgumentException(int invalidQty)
    {
        // Arrange
        var basket = Basket.Create(Guid.NewGuid());

        // Act
        Action act = () => basket.AddBasketProductItem(Guid.NewGuid(), invalidQty, 10m, "Cable");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RemoveBasketProductItem_ShouldRemoveTargetProduct()
    {
        // Arrange
        var basket = Basket.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();
        basket.AddBasketProductItem(productId, 2, 10m, "Item A");

        // Act
        basket.RemoveBasketProductItem(productId);

        // Assert
        basket.BasketProductItems.Should().BeEmpty();
        basket.TotalPrice.Should().Be(0m);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var basket = Basket.Create(Guid.NewGuid());
        basket.AddBasketProductItem(Guid.NewGuid(), 1, 10m, "Item A");
        basket.AddBasketProductItem(Guid.NewGuid(), 2, 25m, "Item B");

        // Act
        basket.Clear();

        // Assert
        basket.BasketProductItems.Should().BeEmpty();
        basket.TotalPrice.Should().Be(0m);
    }
}
