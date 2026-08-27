using Microsoft.AspNetCore.Identity;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Infrastructure.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
            => _hasher.HashPassword(new object(), password);

        public bool Verify(string password, string passwordHash)
            => _hasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password) != PasswordVerificationResult.Failed;
    }
}
