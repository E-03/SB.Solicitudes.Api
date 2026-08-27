using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common.Results;
using SB.Solicitudes.Domain.Dto;
using SB.Solicitudes.Domain.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Features.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<ResultEntity<LoginResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Correo))
            {
                return ResultEntity<LoginResponse>.Failure(
                    ResultError.Validation(
                        "Auth.CorreoRequired",
                        "El correo es obligatorio.",
                        nameof(request.Correo)));
            }

            if (string.IsNullOrWhiteSpace(request.Contraseña))
            {
                return ResultEntity<LoginResponse>.Failure(
                    ResultError.Validation(
                        "Auth.PasswordRequired",
                        "La contraseña es obligatoria.",
                        nameof(request.Contraseña)));
            }

            var usuario =
                await _unitOfWork.Usuarios
                    .GetByCorreoAsync(
                        request.Correo,
                        cancellationToken);

            if (usuario is null ||
                !usuario.Activo)
            {
                return ResultEntity<LoginResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.InvalidCredentials",
                        "Credenciales inválidas."));
            }

            var passwordValid =
                _passwordHasher.Verify(
                    request.Contraseña,
                    usuario.ContraseñaHash);

            if (!passwordValid)
            {
                return ResultEntity<LoginResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.InvalidCredentials",
                        "Credenciales inválidas."));
            }

            var token =
                _jwtService.GenerateToken(usuario);

            var expiresAt =
                _jwtService.GetExpiration();

            return ResultEntity<LoginResponse>.Success(
                new LoginResponse(
                    token,
                    expiresAt,
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Rol));
        }
    }
}
