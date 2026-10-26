using CleanArchitecture.Domain.AggregatesModels.Orders.Enums;
using CleanArchitecture.Domain.AggregatesModels.Orders.Events;
using CleanArchitecture.Domain.AggregatesModels.Shared;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.AggregatesModels.Orders;

public sealed class Order : BaseEntityRoot
{
    private readonly List<OrderItem> _orderItems = new();

    private Order(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public UserInformation UserInformation { get; private set; } = default!;

    public decimal TotalPrice => OrderItems.Sum(o => o.Quantity * o.Price);

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public static Order Create(Guid userId, UserInformation userInformation, IEnumerable<OrderItemDto> orderItems)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));

        ArgumentNullException.ThrowIfNull(userInformation);
        ArgumentNullException.ThrowIfNull(orderItems);

        var itemsList = orderItems.ToList();
        if (itemsList.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item");

        Order order = new Order(userId)
        {
            UserInformation = userInformation
        };

        foreach (var item in itemsList)
        {
            order.AddOrderItem(item.ProductId, item.Quantity, item.Price);
        }

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.UserId));

        return order;
    }

    public void MarkAsCompleted()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled order");

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed order");

        Status = OrderStatus.Cancelled;
    }

    private void AddOrderItem(Guid productId, int quantity, decimal price)
    {
        _orderItems.Add(OrderItem.Create(Id, productId, quantity, price));
    }
}
