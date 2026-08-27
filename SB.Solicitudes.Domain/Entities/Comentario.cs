using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Comentario : BaseEntity
    {
        private Comentario()
        {
        }

        public Comentario(
            int solicitudId,
            int usuarioId,
            string texto,
            VisibilidadComentario visibilidad)
        {
            SolicitudId = solicitudId;
            UsuarioId = usuarioId;
            Texto = texto;
            Visibilidad = visibilidad;
            Fecha = DateTime.UtcNow;
        }

        public string Texto { get; private set; } = null!;
        public VisibilidadComentario Visibilidad { get; private set; }
        public DateTime Fecha { get; private set; }

        public int SolicitudId { get; private set; }
        public Solicitud Solicitud { get; private set; } = null!;

        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;
    }
}
