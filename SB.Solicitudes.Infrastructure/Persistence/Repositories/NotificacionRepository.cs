using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

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
