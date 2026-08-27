using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Infrastructure.CurrentUser
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return int.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public string? Role =>
            _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(ClaimTypes.Role);

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

        public bool IsInRole(string role)
            => _httpContextAccessor.HttpContext?.User.IsInRole(role) == true;
    }
}
