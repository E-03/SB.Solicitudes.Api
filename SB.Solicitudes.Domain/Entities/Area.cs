using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Area : BaseEntity
    {
        private Area()
        {
        }

        public Area(string nombre)
        {
            Nombre = nombre;
            Activa = true;
        }

        public string Nombre { get; private set; } = null!;
        public bool Activa { get; private set; }

        public ICollection<Solicitud> Solicitudes { get; private set; }
            = new List<Solicitud>();
    }
}
