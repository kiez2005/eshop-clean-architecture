using CleanArchitecture.Application.Common.ApplicationServices.Auth;
using CleanArchitecture.Domain.AggregatesModels.Orders;
using CleanArchitecture.Persistence.Common;

namespace CleanArchitecture.Persistence.Repositories;

public sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext, ICurrentUser currentUser) : base(dbContext, currentUser)
    {
    }
}
