using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        private Usuario()
        {
        }

        public Usuario(
            string nombre,
            string correo,
            string contraseñaHash,
            string rol)
        {
            Nombre = nombre;
            Correo = correo;
            ContraseñaHash = contraseñaHash;
            Rol = rol;
            Activo = true;
        }

        public string Nombre { get; private set; } = null!;
        public string Correo { get; private set; } = null!;
        public string ContraseñaHash { get; private set; } = null!;
        public string Rol { get; private set; } = null!;
        public bool Activo { get; private set; }

        public ICollection<Solicitud> SolicitudesCreadas { get; private set; }
            = new List<Solicitud>();

        public ICollection<Solicitud> SolicitudesAsignadas { get; private set; }
            = new List<Solicitud>();

        public ICollection<Comentario> Comentarios { get; private set; }
            = new List<Comentario>();

        public void Desactivar()
        {
            Activo = false;
        }

        public void Activar()
        {
            Activo = true;
        }
    }
}
