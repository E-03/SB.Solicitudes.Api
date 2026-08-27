using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.EntidadesGubernamentales;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface IEntidadGubernamentalRepository : IGenericRepository<EntidadGubernamental>
    {
        Task<PaginatedResult<EntidadGubernamentalResponse>> GetPagedAsync(
            EntidadGubernamentalFilterRequest filter,
            CancellationToken cancellationToken = default);
    }
}
