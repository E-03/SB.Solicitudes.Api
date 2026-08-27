namespace SB.Solicitudes.Domain.Dto
{
    public sealed record ComentarioResponse(
        int Id,
        string Texto,
        string Visibilidad,
        DateTime Fecha,
        int UsuarioId,
        string Usuario);
}
