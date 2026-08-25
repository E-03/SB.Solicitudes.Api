using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class ComentarioRepository
        : GenericRepository<Comentario>, IComentarioRepository
    {
        public ComentarioRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
