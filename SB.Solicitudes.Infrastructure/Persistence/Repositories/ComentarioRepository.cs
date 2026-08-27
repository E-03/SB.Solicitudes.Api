using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

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
