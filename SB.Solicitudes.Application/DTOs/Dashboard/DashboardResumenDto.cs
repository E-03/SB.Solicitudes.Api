using SB.Solicitudes.Application.DTOs.Solicitudes;

namespace SB.Solicitudes.Application.DTOs.Dashboard
{
    public sealed record DashboardResumenDto(
        int SolicitudesAbiertas,
        int SolicitudesCerradas,
        int SolicitudesVencidas,
        IReadOnlyDictionary<string, int> PorEstado,
        IReadOnlyDictionary<string, int> PorPrioridad,
        IReadOnlyCollection<SolicitudListItem> Ultimas);
}
