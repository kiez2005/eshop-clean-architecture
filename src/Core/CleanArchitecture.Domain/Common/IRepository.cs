using Ardalis.Specification;

namespace CleanArchitecture.Domain.Common;

public interface IRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : BaseEntity
{
    Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
