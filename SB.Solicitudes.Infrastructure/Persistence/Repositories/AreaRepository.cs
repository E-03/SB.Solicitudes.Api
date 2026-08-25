using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

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
