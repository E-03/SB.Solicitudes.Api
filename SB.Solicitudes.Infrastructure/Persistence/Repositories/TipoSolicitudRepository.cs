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
    }
}
