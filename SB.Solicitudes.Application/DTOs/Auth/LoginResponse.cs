namespace SB.Solicitudes.Application.DTOs.Auth
{
    public sealed record LoginResponse(
        string Token,
        DateTime ExpiresAt,
        int UsuarioId,
        string Nombre,
        string Correo,
        string Rol);
}
