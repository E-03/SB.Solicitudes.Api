using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

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
