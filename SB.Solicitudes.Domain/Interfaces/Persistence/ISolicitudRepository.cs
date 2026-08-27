using SB.Solicitudes.Domain.Common.Pagination;
using SB.Solicitudes.Domain.Dto;
using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Application.Interfaces.Persistence
{
    public interface ISolicitudRepository : IGenericRepository<Solicitud>
    {
        Task<PaginatedResult<SolicitudListItem>>
           GetPagedAsync(
               SolicitudFilterRequest filter,
               CancellationToken cancellationToken);

        Task<SolicitudResponse> CrearSolicitudAsync(CrearSolicitudRequest request, int usuarioSolicitanteId);

    }
}
