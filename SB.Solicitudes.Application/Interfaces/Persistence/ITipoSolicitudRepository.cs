using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface ITipoSolicitudRepository : IGenericRepository<TipoSolicitud>
    {
        Task<IReadOnlyCollection<TipoSolicitud>> GetActivosAsync(
            CancellationToken cancellationToken = default);
    }
}
