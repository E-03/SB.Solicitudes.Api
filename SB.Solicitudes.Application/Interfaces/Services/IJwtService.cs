using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);

        DateTime GetExpiration();
    }
}
