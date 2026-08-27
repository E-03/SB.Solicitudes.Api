using SB.Solicitudes.Application.DTOs.Comentarios;
using SB.Solicitudes.Application.DTOs.HistorialEstados;

namespace SB.Solicitudes.Application.DTOs.Solicitudes
{
    public sealed record SolicitudDetailResponse(
        int Id,
        string Codigo,
        string Titulo,
        string Descripcion,
        string Prioridad,
        string Estado,
        string Area,
        string TipoSolicitud,
        string UsuarioSolicitante,
        string? Responsable,
        DateTime FechaCreacion,
        DateTime FechaCompromiso,
        DateTime? FechaCierre,
        string? UrlEvidencia,
        string? ReferenciaEvidencia,
        IReadOnlyCollection<ComentarioResponse> Comentarios,
        IReadOnlyCollection<HistorialEstadoResponse> Historial);
}
