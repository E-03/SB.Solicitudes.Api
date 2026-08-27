using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface IHistorialEstadoRepository : IGenericRepository<HistorialEstado>
    {
        Task<IReadOnlyCollection<HistorialEstado>> GetBySolicitudIdAsync(
            int solicitudId,
            CancellationToken cancellationToken = default);
    }
}
