namespace SB.Solicitudes.Application.DTOs.Solicitudes
{
    public sealed record SolicitudListItem(
        int Id,
        string Codigo,
        string Titulo,
        string Prioridad,
        string Estado,
        string Area,
        string TipoSolicitud,
        string UsuarioSolicitante,
        string? Responsable,
        DateTime FechaCreacion,
        DateTime FechaCompromiso);
}
