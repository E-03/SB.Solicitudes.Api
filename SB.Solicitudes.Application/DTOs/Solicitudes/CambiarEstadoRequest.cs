namespace SB.Solicitudes.Application.DTOs.Solicitudes
{
    public sealed record CambiarEstadoRequest(
        string Estado,
        string Comentario);
}
