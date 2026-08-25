using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Area : BaseEntity
    {
        public string Nombre { get; set; }
        public bool Activa { get; set; }

        public ICollection<Solicitud> Solicitudes { get; set; }
    }
}
