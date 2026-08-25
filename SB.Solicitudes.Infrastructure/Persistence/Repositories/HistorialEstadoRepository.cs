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
    }
}
