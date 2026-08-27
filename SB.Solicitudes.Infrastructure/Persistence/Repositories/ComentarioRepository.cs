using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class ComentarioRepository
        : GenericRepository<Comentario>, IComentarioRepository
    {
        public ComentarioRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Comentario>> GetBySolicitudIdAsync(
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

        public Task<bool> ExisteComentarioResolucionAsync(
            int solicitudId,
            CancellationToken cancellationToken = default)
        {
            return DbSet.AnyAsync(
                x => x.SolicitudId == solicitudId &&
                     x.Visibilidad == VisibilidadComentario.Interno &&
                     x.Texto.Contains("Resolución"),
                cancellationToken);
        }
    }
}
