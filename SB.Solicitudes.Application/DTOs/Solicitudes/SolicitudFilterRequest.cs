using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Application.DTOs.Solicitudes
{
    public sealed record SolicitudFilterRequest(
        EstadosSolicitud? Estado,
        PrioridadesSolicitud? Prioridad,
        int? AreaId,
        int? UsuarioSolicitanteId,
        int? ResponsableId,
        DateTime? FechaDesde,
        DateTime? FechaHasta,
        int PageNumber = 1,
        int PageSize = 10);
}
