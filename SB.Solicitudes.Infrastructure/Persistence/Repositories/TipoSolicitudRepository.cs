using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class TipoSolicitudRepository
        : GenericRepository<TipoSolicitud>, ITipoSolicitudRepository
    {
        public TipoSolicitudRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyCollection<TipoSolicitud>> GetActivosAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);
        }
    }
}
