using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Solicitud : BaseEntity
    {
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public PrioridadesSolicitud? Prioridad { get; set; }
        public EstadosSolicitud? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? UrlEvidencia { get; set; }
        public string? ReferenciaEvidencia { get; set; }

        // Relaciones
        public int UsuarioSolicitanteId { get; set; }
        public Usuario UsuarioSolicitante { get; set; }

        public int? ResponsableId { get; set; }
        public Usuario? Responsable { get; set; }

        public int AreaId { get; set; }
        public Area Area { get; set; }

        public int TipoSolicitudId { get; set; }
        public TipoSolicitud TipoSolicitud { get; set; }

        // Colecciones
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    }
}
