using FluentValidation;
using SB.Solicitudes.Application.Common.Extensions;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Comentarios;
using SB.Solicitudes.Application.DTOs.HistorialEstados;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Features.Solicitudes
{
    public sealed class SolicitudService : ISolicitudService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly INotificacionService _notificationService;
        private readonly IValidator<CrearSolicitudRequest> _crearValidator;
        private readonly IValidator<CambiarEstadoRequest> _cambiarEstadoValidator;
        private readonly IValidator<AsignarSolicitudRequest> _asignarValidator;
        private readonly IValidator<CrearComentarioRequest> _comentarioValidator;

        public SolicitudService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            INotificacionService notificationService,
            IValidator<CrearSolicitudRequest> crearValidator,
            IValidator<CambiarEstadoRequest> cambiarEstadoValidator,
            IValidator<AsignarSolicitudRequest> asignarValidator,
            IValidator<CrearComentarioRequest> comentarioValidator)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _notificationService = notificationService;
            _crearValidator = crearValidator;
            _cambiarEstadoValidator = cambiarEstadoValidator;
            _asignarValidator = asignarValidator;
            _comentarioValidator = comentarioValidator;
        }

        public async Task<ResultEntity<PaginatedResult<SolicitudListItem>>> GetPagedAsync(
            SolicitudFilterRequest filter,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated ||
                _currentUser.UserId is null ||
                _currentUser.Role is null)
            {
                return ResultEntity<PaginatedResult<SolicitudListItem>>.Failure(
                    ResultError.Unauthorized(
                        "Auth.Required",
                        "Debe autenticarse."));
            }

            var paged = await _unitOfWork.Solicitudes.GetPagedAsync(
                filter,
                _currentUser.UserId.Value,
                _currentUser.Role,
                cancellationToken);

            return ResultEntity<PaginatedResult<SolicitudListItem>>.Success(paged);
        }

        public async Task<ResultEntity<SolicitudDetailResponse>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            {
                return ResultEntity<SolicitudDetailResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.Required",
                        "Debe autenticarse."));
            }

            var solicitud = await _unitOfWork.Solicitudes
                .GetDetailAsync(id, cancellationToken);

            if (solicitud is null)
            {
                return ResultEntity<SolicitudDetailResponse>.Failure(
                    ResultError.NotFound(
                        "Solicitud.NotFound",
                        "Solicitud no encontrada."));
            }

            if (!PuedeConsultar(solicitud))
            {
                return ResultEntity<SolicitudDetailResponse>.Failure(
                    ResultError.Forbidden(
                        "Solicitud.Forbidden",
                        "No tiene permisos para consultar esta solicitud."));
            }

            var comentarios = await _unitOfWork.Comentarios
                .GetBySolicitudIdAsync(id, cancellationToken);

            var historial = await _unitOfWork.HistorialEstados
                .GetBySolicitudIdAsync(id, cancellationToken);

            return ResultEntity<SolicitudDetailResponse>.Success(
                MapDetail(solicitud, comentarios, historial));
        }

        public async Task<ResultEntity<SolicitudResponse>> CreateAsync(
            CrearSolicitudRequest request,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            {
                return ResultEntity<SolicitudResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.Required",
                        "Debe autenticarse."));
            }

            var crearValidation = await _crearValidator.ValidateAsync(request, cancellationToken);

            if (!crearValidation.IsValid)
            {
                return ResultEntity<SolicitudResponse>.Failure(
                    crearValidation.ToResultErrors());
            }

            if (!await _unitOfWork.Areas.ExistsAsync(request.AreaId, cancellationToken))
            {
                return ResultEntity<SolicitudResponse>.Failure(
                    ResultError.Validation(
                        "Solicitud.AreaNotFound",
                        "El área no existe.",
                        nameof(request.AreaId)));
            }

            if (!await _unitOfWork.TiposSolicitud.ExistsAsync(request.TipoSolicitudId, cancellationToken))
            {
                return ResultEntity<SolicitudResponse>.Failure(
                    ResultError.Validation(
                        "Solicitud.TipoNotFound",
                        "El tipo de solicitud no existe.",
                        nameof(request.TipoSolicitudId)));
            }

            if (!Enum.TryParse<PrioridadesSolicitud>(request.Prioridad, true, out var prioridad))
            {
                return ResultEntity<SolicitudResponse>.Failure(
                    ResultError.Validation(
                        "Solicitud.InvalidPriority",
                        "Prioridad inválida.",
                        nameof(request.Prioridad)));
            }

            var codigo = await GenerarCodigoAsync(cancellationToken);

            var solicitud = new Solicitud(
                codigo,
                request.Titulo,
                request.Descripcion,
                prioridad,
                request.FechaCompromiso,
                _currentUser.UserId.Value,
                request.AreaId,
                request.TipoSolicitudId,
                request.UrlEvidencia,
                request.ReferenciaEvidencia);

            await _unitOfWork.Solicitudes.AddAsync(solicitud, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.EnviarNotificacionCreacionSolicitudAsync(
                solicitud,
                cancellationToken);

            var creada = await _unitOfWork.Solicitudes
                .GetDetailAsync(solicitud.Id, cancellationToken);

            return ResultEntity<SolicitudResponse>.Success(MapResponse(creada!));
        }

        public async Task<Result> ChangeStateAsync(
            int id,
            CambiarEstadoRequest request,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            {
                return Result.Failure(
                    ResultError.Unauthorized(
                        "Auth.Required",
                        "Debe autenticarse."));
            }

            var solicitud = await _unitOfWork.Solicitudes
                .GetByIdAsync(id, cancellationToken);

            if (solicitud is null)
            {
                return Result.Failure(
                    ResultError.NotFound(
                        "Solicitud.NotFound",
                        "Solicitud no encontrada."));
            }

            if (!PuedeConsultar(solicitud))
            {
                return Result.Failure(
                    ResultError.Forbidden(
                        "Solicitud.Forbidden",
                        "No tiene permisos sobre esta solicitud."));
            }

            var estadoValidation = await _cambiarEstadoValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!estadoValidation.IsValid)
            {
                return Result.Failure(estadoValidation.ToResultErrors());
            }

            Enum.TryParse<EstadosSolicitud>(request.Estado, true, out var nuevoEstado);

            if (nuevoEstado == EstadosSolicitud.Cerrada)
            {
                var tieneResolucion = await _unitOfWork.Comentarios
                    .ExisteComentarioResolucionAsync(id, cancellationToken);

                if (!tieneResolucion)
                {
                    return Result.Failure(
                        ResultError.Validation(
                            "Solicitud.ResolutionRequired",
                            "No se puede cerrar la solicitud sin un comentario de resolución."));
                }
            }

            if (solicitud.Estado == EstadosSolicitud.Cerrada &&
                nuevoEstado != EstadosSolicitud.Cerrada &&
                !_currentUser.IsInRole(Roles.Administrador) &&
                !_currentUser.IsInRole(Roles.Analista))
            {
                return Result.Failure(
                    ResultError.Forbidden(
                        "Solicitud.ReopenForbidden",
                        "Solo Administrador o Analista pueden reabrir una solicitud cerrada."));
            }

            var anterior = solicitud.Estado;

            solicitud.CambiarEstado(nuevoEstado);

            var historial = new HistorialEstado(
                solicitud.Id,
                _currentUser.UserId.Value,
                anterior,
                nuevoEstado,
                request.Comentario);

            await _unitOfWork.HistorialEstados.AddAsync(historial, cancellationToken);

            _unitOfWork.Solicitudes.Update(solicitud);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (nuevoEstado == EstadosSolicitud.Cerrada)
            {
                await _notificationService.EnviarNotificacionCierreAsync(
                    solicitud,
                    cancellationToken);
            }
            else
            {
                await _notificationService.EnviarNotificacionCambioEstadoAsync(
                    solicitud,
                    anterior.ToString(),
                    nuevoEstado.ToString(),
                    cancellationToken);
            }

            return Result.Success();
        }

        public async Task<Result> AssignAsync(
            int id,
            AsignarSolicitudRequest request,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInRole(Roles.Administrador) &&
                !_currentUser.IsInRole(Roles.Analista))
            {
                return Result.Failure(
                    ResultError.Forbidden(
                        "Solicitud.AssignForbidden",
                        "No tiene permisos para asignar solicitudes."));
            }

            var asignarValidation = await _asignarValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!asignarValidation.IsValid)
            {
                return Result.Failure(asignarValidation.ToResultErrors());
            }

            var solicitud = await _unitOfWork.Solicitudes
                .GetByIdAsync(id, cancellationToken);

            if (solicitud is null)
            {
                return Result.Failure(
                    ResultError.NotFound(
                        "Solicitud.NotFound",
                        "Solicitud no encontrada."));
            }

            if (request.ResponsableId is null)
            {
                solicitud.AsignarResponsable(null);
            }
            else
            {
                var responsable = await _unitOfWork.Usuarios
                    .GetByIdAsync(request.ResponsableId.Value, cancellationToken);

                if (responsable is null ||
                    !responsable.Activo ||
                    responsable.Rol != Roles.Analista)
                {
                    return Result.Failure(
                        ResultError.Validation(
                            "Solicitud.ResponsableInvalid",
                            "El responsable debe ser un analista activo.",
                            nameof(request.ResponsableId)));
                }

                solicitud.AsignarResponsable(responsable.Id);
            }

            _unitOfWork.Solicitudes.Update(solicitud);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (solicitud.ResponsableId is not null)
            {
                await _notificationService.EnviarNotificacionAsignacionAsync(
                    solicitud,
                    cancellationToken);
            }

            return Result.Success();
        }

        public async Task<ResultEntity<ComentarioResponse>> AddCommentAsync(
            int id,
            CrearComentarioRequest request,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            {
                return ResultEntity<ComentarioResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.Required",
                        "Debe autenticarse."));
            }

            var solicitud = await _unitOfWork.Solicitudes
                .GetByIdAsync(id, cancellationToken);

            if (solicitud is null)
            {
                return ResultEntity<ComentarioResponse>.Failure(
                    ResultError.NotFound(
                        "Solicitud.NotFound",
                        "Solicitud no encontrada."));
            }

            if (!PuedeConsultar(solicitud))
            {
                return ResultEntity<ComentarioResponse>.Failure(
                    ResultError.Forbidden(
                        "Solicitud.Forbidden",
                        "No tiene permisos sobre esta solicitud."));
            }

            var comentarioValidation = await _comentarioValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!comentarioValidation.IsValid)
            {
                return ResultEntity<ComentarioResponse>.Failure(
                    comentarioValidation.ToResultErrors());
            }

            Enum.TryParse<VisibilidadComentario>(request.Visibilidad, true, out var visibilidad);

            if (visibilidad == VisibilidadComentario.Interno &&
                !_currentUser.IsInRole(Roles.Administrador) &&
                !_currentUser.IsInRole(Roles.Analista))
            {
                return ResultEntity<ComentarioResponse>.Failure(
                    ResultError.Forbidden(
                        "Comentario.InternalForbidden",
                        "Solo Administrador o Analista pueden crear comentarios internos."));
            }

            var comentario = new Comentario(
                id,
                _currentUser.UserId.Value,
                request.Texto,
                visibilidad);

            await _unitOfWork.Comentarios.AddAsync(comentario, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var usuario = await _unitOfWork.Usuarios
                .GetByIdAsync(_currentUser.UserId.Value, cancellationToken);

            return ResultEntity<ComentarioResponse>.Success(
                new ComentarioResponse(
                    comentario.Id,
                    comentario.Texto,
                    comentario.Visibilidad.ToString(),
                    comentario.Fecha,
                    comentario.UsuarioId,
                    usuario?.Nombre ?? string.Empty));
        }

        private bool PuedeConsultar(Solicitud solicitud)
        {
            if (_currentUser.IsInRole(Roles.Administrador))
            {
                return true;
            }

            if (_currentUser.IsInRole(Roles.Analista))
            {
                return solicitud.ResponsableId is null ||
                       solicitud.ResponsableId == _currentUser.UserId;
            }

            return solicitud.UsuarioSolicitanteId == _currentUser.UserId;
        }

        private async Task<string> GenerarCodigoAsync(
            CancellationToken cancellationToken)
        {
            var ultimo = await _unitOfWork.Solicitudes
                .ObtenerUltimoCodigoAsync(cancellationToken);

            var numero = 1;

            if (!string.IsNullOrWhiteSpace(ultimo))
            {
                var partes = ultimo.Split('-');

                if (partes.Length == 3 && int.TryParse(partes[2], out var actual))
                {
                    numero = actual + 1;
                }
            }

            return $"SOL-{DateTime.UtcNow.Year}-{numero:D4}";
        }

        private static SolicitudResponse MapResponse(Solicitud solicitud)
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

        private static SolicitudDetailResponse MapDetail(
            Solicitud solicitud,
            IReadOnlyCollection<Comentario> comentarios,
            IReadOnlyCollection<HistorialEstado> historial)
        {
            return new SolicitudDetailResponse(
                solicitud.Id,
                solicitud.Codigo,
                solicitud.Titulo,
                solicitud.Descripcion,
                solicitud.Prioridad.ToString(),
                solicitud.Estado.ToString(),
                solicitud.Area.Nombre,
                solicitud.TipoSolicitud.Nombre,
                solicitud.UsuarioSolicitante.Nombre,
                solicitud.Responsable?.Nombre,
                solicitud.FechaCreacion,
                solicitud.FechaCompromiso,
                solicitud.FechaCierre,
                solicitud.UrlEvidencia,
                solicitud.ReferenciaEvidencia,
                comentarios
                    .Select(c => new ComentarioResponse(
                        c.Id,
                        c.Texto,
                        c.Visibilidad.ToString(),
                        c.Fecha,
                        c.UsuarioId,
                        c.Usuario?.Nombre ?? string.Empty))
                    .ToList(),
                historial
                    .Select(h => new HistorialEstadoResponse(
                        h.Id,
                        h.EstadoAnterior.ToString(),
                        h.EstadoNuevo.ToString(),
                        h.Fecha,
                        h.UsuarioId,
                        h.Usuario?.Nombre ?? string.Empty,
                        h.Comentario))
                    .ToList());
        }
    }
}
