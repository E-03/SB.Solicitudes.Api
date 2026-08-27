using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Common.Pagination;
using SB.Solicitudes.Domain.Common.Results;
using SB.Solicitudes.Domain.Dto;
using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Features.Solicitudes
{
    public sealed class SolicitudService : ISolicitudService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificacionService _notificationService;

        public SolicitudService(
            IUnitOfWork unitOfWork,
            INotificacionService notificationService
            )
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public Task<ResultEntity<ComentarioResponse>> AddCommentAsync(int id, CrearComentarioRequest request, int currentUserId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result> AssignAsync(int id, AsignarSolicitudRequest request, int currentUserId, string currentUserRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ChangeStateAsync(int id, CambiarEstadoRequest request, int currentUserId, string currentUserRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<SolicitudResponse> CreateAsync(CrearSolicitudRequest request, 
            int usuarioSolicitanteId, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<PrioridadesSolicitud>(request.Prioridad, out var prioridad))
            {
                throw new InvalidOperationException("Prioridad no válida");
            }

            var codigo = GenerarCodigoSolicitud();

            var solicitud = new Solicitud
            {
                Codigo = codigo,
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                Prioridad = prioridad,
                Estado = EstadosSolicitud.Registrada,
                FechaCreacion = DateTime.UtcNow,
                FechaCompromiso = request.FechaCompromiso,
                UrlEvidencia = request.UrlEvidencia,
                ReferenciaEvidencia = request.ReferenciaEvidencia,
                UsuarioSolicitanteId = usuarioSolicitanteId,
                AreaId = request.AreaId,
                TipoSolicitudId = request.TipoSolicitudId
            };

            await _unitOfWork.Solicitudes.AddAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.EnviarNotificacionCreacionSolicitudAsync(solicitud);

            return MapearResponse(solicitud);
        }

        public Task<ResultEntity<SolicitudDetailResponse>> GetByIdAsync(int id, int currentUserId, string currentUserRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultEntity<PaginatedResult<SolicitudListItem>>> GetPagedAsync(SolicitudFilterRequest filter, int currentUserId, string currentUserRole, CancellationToken cancellationToken)
        {
            var solicitudes = await _unitOfWork.Solicitudes.GetPagedAsync(filter, cancellationToken);
            return ResultEntity<PaginatedResult<SolicitudListItem>>.Success(solicitudes);
        }

        private string GenerarCodigoSolicitud()
        {
            var año = DateTime.UtcNow.Year;
            var random = new Random().Next(1, 10000).ToString("D4");
            return $"SOL-{año}-{random}";
        }

        private SolicitudResponse MapearResponse(Solicitud solicitud)
        {
            return new SolicitudResponse
            {
                Id = solicitud.Id,
                Codigo = solicitud.Codigo,
                Titulo = solicitud.Titulo,
                Descripcion = solicitud.Descripcion,
                Prioridad = solicitud.Prioridad.ToString(),
                Estado = solicitud.Estado.ToString(),
                FechaCreacion = solicitud.FechaCreacion,
                FechaCompromiso = solicitud.FechaCompromiso,
                FechaCierre = solicitud.FechaCierre,
                UrlEvidencia = solicitud.UrlEvidencia,
                ReferenciaEvidencia = solicitud.ReferenciaEvidencia,
                UsuarioSolicitanteId = solicitud.UsuarioSolicitanteId,
                UsuarioSolicitanteNombre = solicitud.UsuarioSolicitante?.Nombre,
                ResponsableId = solicitud.ResponsableId,
                ResponsableNombre = solicitud.Responsable?.Nombre,
                AreaId = solicitud.AreaId,
                AreaNombre = solicitud.Area?.Nombre,
                TipoSolicitudId = solicitud.TipoSolicitudId,
                TipoSolicitudNombre = solicitud.TipoSolicitud?.Nombre
            };
        }
    }
}
