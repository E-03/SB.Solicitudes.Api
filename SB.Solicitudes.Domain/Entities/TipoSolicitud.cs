using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class TipoSolicitud : BaseEntity
    {
        private TipoSolicitud()
        {
        }

        public TipoSolicitud(string nombre, string? descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = true;
        }

        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; }

        public ICollection<Solicitud> Solicitudes { get; private set; }
            = new List<Solicitud>();
    }
}
