namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }

        string? Role { get; }

        bool IsAuthenticated { get; }

        bool IsInRole(string role);
    }
}
