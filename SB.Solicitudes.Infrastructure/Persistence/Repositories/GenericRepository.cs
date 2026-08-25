using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Common.Pagination;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity>
        : IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericRepository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(
                    entity => entity.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<PaginatedResult<TEntity>> GetPagedAsync(
            PaginationRequest pagination,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet
                .AsNoTracking();

            var totalCount = await query
                .CountAsync(cancellationToken);

            var items = await query
                .Skip(pagination.Skip)
                .Take(pagination.ValidPageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TEntity>(
                items,
                pagination.ValidPageNumber,
                pagination.ValidPageSize,
                totalCount);
        }

        public async Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(
                entity,
                cancellationToken);
        }

        public async Task AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(
                entities,
                cancellationToken);
        }

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AnyAsync(
                    entity => entity.Id == id,
                    cancellationToken);
        }
    }
}