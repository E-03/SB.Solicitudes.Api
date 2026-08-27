namespace SB.Solicitudes.Application.DTOs.Comentarios
{
    public sealed record CrearComentarioRequest(
        string Texto,
        string Visibilidad);
}
