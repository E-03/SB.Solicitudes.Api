using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class AreaRepository
        : GenericRepository<Area>, IAreaRepository
    {
        public AreaRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Area>> GetActivasAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(x => x.Activa)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);
        }
    }
}
