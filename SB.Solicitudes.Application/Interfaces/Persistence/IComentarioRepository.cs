using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface IComentarioRepository : IGenericRepository<Comentario>
    {
        Task<IReadOnlyCollection<Comentario>> GetBySolicitudIdAsync(
            int solicitudId,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteComentarioResolucionAsync(
            int solicitudId,
            CancellationToken cancellationToken = default);
    }
}
