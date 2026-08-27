namespace SB.Solicitudes.Domain.Dto
{
    public sealed record CrearComentarioRequest(
        string Texto,
        string Visibilidad);
}
