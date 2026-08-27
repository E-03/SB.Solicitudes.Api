using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Correo == correo,
                    cancellationToken);
        }
    }
}
