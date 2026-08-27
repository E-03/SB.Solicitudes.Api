namespace SB.Solicitudes.Application.DTOs.EntidadesGubernamentales
{
    public sealed record EntidadGubernamentalResponse(
        int Id,
        string Nombre,
        string Categoria,
        string PoderDelEstado,
        string Sector);
}
