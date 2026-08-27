using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Auth;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResultEntity<LoginResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken);
    }
}
