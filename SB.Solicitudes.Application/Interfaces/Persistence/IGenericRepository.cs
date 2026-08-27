using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Application.Interfaces.Persistence
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
