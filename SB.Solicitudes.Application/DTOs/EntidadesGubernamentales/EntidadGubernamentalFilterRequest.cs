namespace SB.Solicitudes.Application.DTOs.EntidadesGubernamentales
{
    public sealed record EntidadGubernamentalFilterRequest(
        string? Search,
        int PageNumber = 1,
        int PageSize = 10);
}
