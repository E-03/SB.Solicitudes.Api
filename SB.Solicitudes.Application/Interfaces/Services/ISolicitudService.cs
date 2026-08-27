using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Comentarios;
using SB.Solicitudes.Application.DTOs.Solicitudes;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface ISolicitudService
    {
        Task<ResultEntity<PaginatedResult<SolicitudListItem>>> GetPagedAsync(
            SolicitudFilterRequest filter,
            CancellationToken cancellationToken);

        Task<ResultEntity<SolicitudDetailResponse>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken);

        Task<ResultEntity<SolicitudResponse>> CreateAsync(
            CrearSolicitudRequest request,
            CancellationToken cancellationToken);

        Task<Result> ChangeStateAsync(
            int id,
            CambiarEstadoRequest request,
            CancellationToken cancellationToken);

        Task<Result> AssignAsync(
            int id,
            AsignarSolicitudRequest request,
            CancellationToken cancellationToken);

        Task<ResultEntity<ComentarioResponse>> AddCommentAsync(
            int id,
            CrearComentarioRequest request,
            CancellationToken cancellationToken);
    }
}
