using CleanArchitecture.Application.Common.ApplicationServices.Auth;
using CleanArchitecture.Application.Common.ApplicationServices.FileStorage;
using CleanArchitecture.Application.Features.V1.Baskets.Commands.CheckoutBasket;
using CleanArchitecture.Application.Features.V1.Baskets.Models.Requests;
using CleanArchitecture.Application.Features.V1.Products.Commands.CreateProduct;
using CleanArchitecture.Domain.AggregatesModels.Baskets;
using CleanArchitecture.Domain.AggregatesModels.Categories;
using CleanArchitecture.Domain.AggregatesModels.Orders;
using CleanArchitecture.Domain.AggregatesModels.Products;
using CleanArchitecture.Domain.Services.Checkout;
using FluentAssertions;
using NSubstitute;

namespace CleanArchitecture.Application.UnitTests.Application;

public sealed class CommandHandlerTests
{
    [Fact]
    public async Task CreateProductCommandHandler_WhenCategoryDoesNotExist_ShouldReturnCategoryNotFoundFailure()
    {
        // Arrange
        var productRepo = Substitute.For<IProductRepository>();
        var categoryRepo = Substitute.For<ICategoryRepository>();
        var fileStorage = Substitute.For<IFileStorageService>();

        categoryRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var handler = new CreateProductCommandHandler(productRepo, categoryRepo, fileStorage);
        var command = new CreateProductCommand("Mechanical Keyboard", "RGB", 99m, null, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task CheckoutCommandHandler_WhenBasketIsEmpty_ShouldReturnBasketEmptyFailure()
    {
        // Arrange
        var checkoutService = new CheckoutService();
        var basketRepo = Substitute.For<IBasketRepository>();
        var orderRepo = Substitute.For<IOrderRepository>();
        var currentUser = Substitute.For<ICurrentUser>();

        currentUser.GetUserId().Returns(Guid.NewGuid());
        basketRepo.FirstOrDefaultAsync(Arg.Any<Ardalis.Specification.ISpecification<Basket>>(), Arg.Any<CancellationToken>())
            .Returns((Basket?)null);

        var handler = new CheckoutCommandHandler(checkoutService, basketRepo, orderRepo, currentUser);
        var command = new CheckoutCommand(
            new UserInformationRequest(
                "Trung",
                "0912345678",
                new AddressRequest("123 Street", "HCMC")));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BasketErrors.BasketProductItemEmpty);
    }
}
