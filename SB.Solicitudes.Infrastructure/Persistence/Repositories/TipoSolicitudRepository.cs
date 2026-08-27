using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class TipoSolicitudRepository
        : GenericRepository<TipoSolicitud>, ITipoSolicitudRepository
    {
        public TipoSolicitudRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
