using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Domain.Entities
{
    public class EntidadGubernamental : BaseEntity
    {
        private EntidadGubernamental()
        {
        }

        public EntidadGubernamental(
            string nombre,
            string categoria,
            string poderDelEstado,
            string sector)
        {
            Nombre = nombre;
            Categoria = categoria;
            PoderDelEstado = poderDelEstado;
            Sector = sector;
        }

        public string Nombre { get; private set; } = null!;
        public string Categoria { get; private set; } = null!;
        public string PoderDelEstado { get; private set; } = null!;
        public string Sector { get; private set; } = null!;

        public void Actualizar(
            string nombre,
            string categoria,
            string poderDelEstado,
            string sector)
        {
            Nombre = nombre;
            Categoria = categoria;
            PoderDelEstado = poderDelEstado;
            Sector = sector;
        }
    }
}
