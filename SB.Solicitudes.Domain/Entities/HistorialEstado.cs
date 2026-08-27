using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class HistorialEstado : BaseEntity
    {
        private HistorialEstado()
        {
        }

        public HistorialEstado(
            int solicitudId,
            int usuarioId,
            EstadosSolicitud estadoAnterior,
            EstadosSolicitud estadoNuevo,
            string comentario)
        {
            SolicitudId = solicitudId;
            UsuarioId = usuarioId;
            EstadoAnterior = estadoAnterior;
            EstadoNuevo = estadoNuevo;
            Comentario = comentario;
            Fecha = DateTime.UtcNow;
        }

        public EstadosSolicitud EstadoAnterior { get; private set; }
        public EstadosSolicitud EstadoNuevo { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Comentario { get; private set; } = null!;

        public int SolicitudId { get; private set; }
        public Solicitud Solicitud { get; private set; } = null!;

        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;
    }
}
