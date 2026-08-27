using SB.Solicitudes.Domain.Common.Pagination;
using SB.Solicitudes.Domain.Common.Results;
using SB.Solicitudes.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface ISolicitudService
    {
        Task<ResultEntity<PaginatedResult<SolicitudListItem>>>
            GetPagedAsync(
                SolicitudFilterRequest filter,
                int currentUserId,
                string currentUserRole,
                CancellationToken cancellationToken);

        Task<ResultEntity<SolicitudDetailResponse>>
            GetByIdAsync(
                int id,
                int currentUserId,
                string currentUserRole,
                CancellationToken cancellationToken);

        Task<SolicitudResponse>
             CreateAsync(
                CrearSolicitudRequest request,
                int currentUserId,
                CancellationToken cancellationToken);

        Task<Result>
            ChangeStateAsync(
                int id,
                CambiarEstadoRequest request,
                int currentUserId,
                string currentUserRole,
                CancellationToken cancellationToken);

        Task<Result>
            AssignAsync(
                int id,
                AsignarSolicitudRequest request,
                int currentUserId,
                string currentUserRole,
                CancellationToken cancellationToken);

        Task<ResultEntity<ComentarioResponse>>
            AddCommentAsync(
                int id,
                CrearComentarioRequest request,
                int currentUserId,
                CancellationToken cancellationToken);
    }
}
