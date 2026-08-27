namespace SB.Solicitudes.Domain.Dto
{
    public sealed record CambiarEstadoRequest(
       string Estado,
       string Comentario);
}
