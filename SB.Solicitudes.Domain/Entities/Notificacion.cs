using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Notificacion : BaseEntity
    {
        public string Canal { get; set; } // email, sms, etc.
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }

        public Guid SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public Guid UsuarioDestinoId { get; set; }
        public Usuario UsuarioDestino { get; set; }
    }

}
