using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Solicitud : BaseEntity
    {
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaCompromiso { get; set; }

        // Relaciones
        public Guid UsuarioSolicitanteId { get; set; }
        public Usuario UsuarioSolicitante { get; set; }

        public Guid ResponsableId { get; set; }
        public Usuario Responsable { get; set; }

        public Guid AreaId { get; set; }
        public Area Area { get; set; }

        public Guid TipoSolicitudId { get; set; }
        public TipoSolicitud TipoSolicitud { get; set; }
    }
}
