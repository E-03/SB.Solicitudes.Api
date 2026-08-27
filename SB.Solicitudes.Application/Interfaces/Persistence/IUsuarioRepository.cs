using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetByCorreoAsync(
            string correo,
            CancellationToken cancellationToken = default);
    }
}
