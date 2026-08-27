using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Notificacion : BaseEntity
    {
        private Notificacion()
        {
        }

        public Notificacion(
            int solicitudId,
            int usuarioDestinoId,
            CanalNotificacion canal,
            string asunto,
            string mensaje)
        {
            SolicitudId = solicitudId;
            UsuarioDestinoId = usuarioDestinoId;
            Canal = canal;
            Asunto = asunto;
            Mensaje = mensaje;
            Estado = EstadoNotificacion.Pendiente;
            Fecha = DateTime.UtcNow;
        }

        public CanalNotificacion Canal { get; private set; }
        public string Asunto { get; private set; } = null!;
        public string Mensaje { get; private set; } = null!;
        public EstadoNotificacion Estado { get; private set; }
        public DateTime Fecha { get; private set; }

        public int SolicitudId { get; private set; }
        public Solicitud Solicitud { get; private set; } = null!;

        public int UsuarioDestinoId { get; private set; }
        public Usuario UsuarioDestino { get; private set; } = null!;

        public void MarcarEnviada()
        {
            Estado = EstadoNotificacion.Enviada;
        }

        public void MarcarFallida()
        {
            Estado = EstadoNotificacion.Fallida;
        }
    }
}
