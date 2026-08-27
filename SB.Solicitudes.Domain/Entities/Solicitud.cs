using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Solicitud : BaseEntity
    {
        private Solicitud()
        {
        }

        public Solicitud(
            string codigo,
            string titulo,
            string descripcion,
            PrioridadesSolicitud prioridad,
            DateTime fechaCompromiso,
            int usuarioSolicitanteId,
            int areaId,
            int tipoSolicitudId,
            string? urlEvidencia,
            string? referenciaEvidencia)
        {
            Codigo = codigo;
            Titulo = titulo;
            Descripcion = descripcion;
            Prioridad = prioridad;
            Estado = EstadosSolicitud.Registrada;
            FechaCreacion = DateTime.UtcNow;
            FechaCompromiso = fechaCompromiso;
            UsuarioSolicitanteId = usuarioSolicitanteId;
            AreaId = areaId;
            TipoSolicitudId = tipoSolicitudId;
            UrlEvidencia = urlEvidencia;
            ReferenciaEvidencia = referenciaEvidencia;
        }

        public string Codigo { get; private set; } = null!;
        public string Titulo { get; private set; } = null!;
        public string Descripcion { get; private set; } = null!;
        public PrioridadesSolicitud Prioridad { get; private set; }
        public EstadosSolicitud Estado { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime FechaCompromiso { get; private set; }
        public DateTime? FechaCierre { get; private set; }
        public string? UrlEvidencia { get; private set; }
        public string? ReferenciaEvidencia { get; private set; }

        public int UsuarioSolicitanteId { get; private set; }
        public Usuario UsuarioSolicitante { get; private set; } = null!;

        public int? ResponsableId { get; private set; }
        public Usuario? Responsable { get; private set; }

        public int AreaId { get; private set; }
        public Area Area { get; private set; } = null!;

        public int TipoSolicitudId { get; private set; }
        public TipoSolicitud TipoSolicitud { get; private set; } = null!;

        public ICollection<Comentario> Comentarios { get; private set; }
            = new List<Comentario>();

        public ICollection<HistorialEstado> HistorialEstados { get; private set; }
            = new List<HistorialEstado>();

        public ICollection<Notificacion> Notificaciones { get; private set; }
            = new List<Notificacion>();

        public void CambiarEstado(EstadosSolicitud nuevoEstado)
        {
            Estado = nuevoEstado;

            FechaCierre = nuevoEstado == EstadosSolicitud.Cerrada
                ? DateTime.UtcNow
                : null;
        }

        public void AsignarResponsable(int? responsableId)
        {
            ResponsableId = responsableId;
        }
    }
}
