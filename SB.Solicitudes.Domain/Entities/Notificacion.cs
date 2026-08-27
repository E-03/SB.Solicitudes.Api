using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Notificacion : BaseEntity
    {
        public CanalNotificacion Canal { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public EstadoNotificacion Estado { get; set; }
        public DateTime Fecha { get; set; }

        public int SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public int UsuarioDestinoId { get; set; }
        public Usuario UsuarioDestino { get; set; }
    }

}
