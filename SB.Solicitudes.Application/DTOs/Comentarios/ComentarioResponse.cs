namespace SB.Solicitudes.Application.DTOs.Comentarios
{
    public sealed record ComentarioResponse(
        int Id,
        string Texto,
        string Visibilidad,
        DateTime Fecha,
        int UsuarioId,
        string Usuario);
}
