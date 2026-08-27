namespace SB.Solicitudes.Application.DTOs.Auth
{
    public sealed record LoginRequest(
        string Correo,
        string Contraseña);
}
