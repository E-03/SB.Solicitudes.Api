using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface IAreaRepository : IGenericRepository<Area>
    {
        Task<IReadOnlyCollection<Area>> GetActivasAsync(
            CancellationToken cancellationToken = default);
    }
}
