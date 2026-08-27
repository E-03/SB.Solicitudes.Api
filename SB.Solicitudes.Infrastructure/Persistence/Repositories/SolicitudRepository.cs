using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class SolicitudRepository
        : GenericRepository<Solicitud>, ISolicitudRepository
    {
        public SolicitudRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
