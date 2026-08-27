using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository
        : GenericRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context)
            : base(context)
        {
        }

        public Task<Usuario?> GetByCorreoAsync(
            string correo,
            CancellationToken cancellationToken = default)
        {
            return DbSet
                .FirstOrDefaultAsync(
                    x => x.Correo == correo,
                    cancellationToken);
        }
    }
}
