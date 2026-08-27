namespace SB.Solicitudes.Application.DTOs.Solicitudes
{
    public sealed record CrearSolicitudRequest(
        string Titulo,
        string Descripcion,
        string Prioridad,
        int AreaId,
        int TipoSolicitudId,
        DateTime FechaCompromiso,
        string? UrlEvidencia,
        string? ReferenciaEvidencia);
}
