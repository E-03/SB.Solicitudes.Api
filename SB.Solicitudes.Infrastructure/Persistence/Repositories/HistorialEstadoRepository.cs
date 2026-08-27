using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class HistorialEstadoRepository
        : GenericRepository<HistorialEstado>, IHistorialEstadoRepository
    {
        public HistorialEstadoRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyCollection<HistorialEstado>> GetBySolicitudIdAsync(
            int solicitudId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(x => x.Usuario)
                .Where(x => x.SolicitudId == solicitudId)
                .OrderBy(x => x.Fecha)
                .ToListAsync(cancellationToken);
        }
    }
}
