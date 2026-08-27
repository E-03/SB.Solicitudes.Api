using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class HistorialEstadoRepository
        : GenericRepository<HistorialEstado>, IHistorialEstadoRepository
    {
        public HistorialEstadoRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
