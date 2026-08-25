using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nombre { get; set; }
        public string ContraseñaHash { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }

        // Relaciones
        public ICollection<Solicitud> SolicitudesCreadas { get; set; }
        public ICollection<Solicitud> SolicitudesAsignadas { get; set; }
        public ICollection<Comentario> Comentarios { get; set; }
    }

}
