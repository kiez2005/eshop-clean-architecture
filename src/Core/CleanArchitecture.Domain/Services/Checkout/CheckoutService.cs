using CleanArchitecture.Domain.AggregatesModels.Baskets;
using CleanArchitecture.Domain.AggregatesModels.Orders;
using CleanArchitecture.Domain.AggregatesModels.Shared;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Services.Checkout;

public class CheckoutService : IDomainService
{
    public Order Checkout(Basket basket, UserInformation userInformation)
    {
        ArgumentNullException.ThrowIfNull(basket);
        ArgumentNullException.ThrowIfNull(userInformation);

        if (basket.BasketProductItems.Count == 0)
            throw new InvalidOperationException("Cannot checkout an empty basket");

        var orderItems = basket.BasketProductItems
            .Select(x => new OrderItemDto(x.ProductId, x.Quantity, x.Product?.Price ?? x.Price))
            .ToList();

        Order order = Order.Create(basket.UserId, userInformation, orderItems);
        basket.Clear();

        return order;
    }
}
