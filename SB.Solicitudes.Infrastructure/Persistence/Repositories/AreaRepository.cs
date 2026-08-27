using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class AreaRepository
        : GenericRepository<Area>, IAreaRepository
    {
        public AreaRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
