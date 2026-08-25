using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class HistorialEstado : BaseEntity
    {
        public string EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }

        public Guid SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
