namespace SB.Solicitudes.Domain.Dto
{
    public sealed record HistorialEstadoResponse(
        int Id,
        string EstadoAnterior,
        string EstadoNuevo,
        DateTime Fecha,
        int UsuarioId,
        string Usuario,
        string? Comentario);
}
