using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Common.Pagination;

namespace SB.Solicitudes.Domain.Interfaces.Persistence
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<TEntity>> GetPagedAsync(
            PaginationRequest pagination,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        Task<bool> ExistsAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}