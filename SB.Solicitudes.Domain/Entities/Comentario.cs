using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Comentario : BaseEntity
    {
        public string Texto { get; set; }
        public string Visibilidad { get; set; } // interno / externo
        public DateTime Fecha { get; set; }

        public Guid SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }





}
