using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class TipoSolicitud : BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }

        public ICollection<Solicitud> Solicitudes { get; set; }
    }


}
