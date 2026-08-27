namespace SB.Solicitudes.Domain.Dto
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
        //string? EvidenciaUrl,
        IReadOnlyCollection<ComentarioResponse> Comentarios,
        IReadOnlyCollection<HistorialEstadoResponse> Historial);
}
