using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class NotificacionRepository
        : GenericRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
