namespace SB.Solicitudes.Application.DTOs.Catalogos
{
    public sealed record TipoSolicitudResponse(
        int Id,
        string Nombre,
        string? Descripcion);
}
