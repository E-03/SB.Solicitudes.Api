using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface ISolicitudRepository : IGenericRepository<Solicitud>
    {
        Task<PaginatedResult<SolicitudListItem>> GetPagedAsync(
            SolicitudFilterRequest filter,
            int currentUserId,
            string currentUserRole,
            CancellationToken cancellationToken = default);

        Task<Solicitud?> GetDetailAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<string?> ObtenerUltimoCodigoAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountAbiertasAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountCerradasAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountVencidasAsync(
            DateTime now,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<string, int>> CountByEstadoGroupedAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<string, int>> CountByPrioridadGroupedAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Solicitud>> GetUltimasAsync(
            int cantidad,
            CancellationToken cancellationToken = default);
    }
}
