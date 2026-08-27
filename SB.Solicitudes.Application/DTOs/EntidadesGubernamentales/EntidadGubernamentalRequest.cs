namespace SB.Solicitudes.Application.DTOs.EntidadesGubernamentales
{
    public sealed record EntidadGubernamentalRequest(
        string Nombre,
        string Categoria,
        string PoderDelEstado,
        string Sector);
}
