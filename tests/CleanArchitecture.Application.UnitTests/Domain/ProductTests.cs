using CleanArchitecture.Domain.AggregatesModels.Products;
using FluentAssertions;

namespace CleanArchitecture.Application.UnitTests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnProduct()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var product = Product.Create("Wireless Mouse", "Ergonomic 2.4GHz mouse", 29.99m, "/images/mouse.png", categoryId);

        // Assert
        product.Name.Should().Be("Wireless Mouse");
        product.Description.Should().Be("Ergonomic 2.4GHz mouse");
        product.Price.Should().Be(29.99m);
        product.ImagePath.Should().Be("/images/mouse.png");
        product.CategoryId.Should().Be(categoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyName_ShouldThrowArgumentException(string invalidName)
    {
        // Act
        Action act = () => Product.Create(invalidName, "Desc", 10m, null, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => Product.Create("Keyboard", "Mechanical", -5m, null, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => Product.Create("Keyboard", "Mechanical", 50m, null, Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var product = Product.Create("Monitor", "4K IPS", 300m, null, Guid.NewGuid());

        // Act
        product.UpdatePrice(259.99m);

        // Assert
        product.Price.Should().Be(259.99m);
    }

    [Fact]
    public void UpdatePrice_WithNegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var product = Product.Create("Monitor", "4K IPS", 300m, null, Guid.NewGuid());

        // Act
        Action act = () => product.UpdatePrice(-1m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
