namespace SB.Solicitudes.Application.DTOs.Solicitudes
{
    public sealed class SolicitudResponse
    {
        public int Id { get; init; }
        public string Codigo { get; init; } = null!;
        public string Titulo { get; init; } = null!;
        public string Descripcion { get; init; } = null!;
        public string Prioridad { get; init; } = null!;
        public string Estado { get; init; } = null!;
        public DateTime FechaCreacion { get; init; }
        public DateTime FechaCompromiso { get; init; }
        public DateTime? FechaCierre { get; init; }
        public string? UrlEvidencia { get; init; }
        public string? ReferenciaEvidencia { get; init; }
        public int UsuarioSolicitanteId { get; init; }
        public string? UsuarioSolicitanteNombre { get; init; }
        public int? ResponsableId { get; init; }
        public string? ResponsableNombre { get; init; }
        public int AreaId { get; init; }
        public string? AreaNombre { get; init; }
        public int TipoSolicitudId { get; init; }
        public string? TipoSolicitudNombre { get; init; }
    }
}
