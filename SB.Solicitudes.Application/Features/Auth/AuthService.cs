using FluentValidation;
using SB.Solicitudes.Application.Common.Extensions;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Auth;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Application.Features.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IValidator<LoginRequest> _validator;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IValidator<LoginRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _validator = validator;
        }

        public async Task<ResultEntity<LoginResponse>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return ResultEntity<LoginResponse>.Failure(
                    validation.ToResultErrors());
            }

            var usuario = await _unitOfWork.Usuarios
                .GetByCorreoAsync(request.Correo, cancellationToken);

            if (usuario is null || !usuario.Activo)
            {
                return ResultEntity<LoginResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.InvalidCredentials",
                        "Credenciales inválidas."));
            }

            var passwordValid = _passwordHasher.Verify(
                request.Contraseña,
                usuario.ContraseñaHash);

            if (!passwordValid)
            {
                return ResultEntity<LoginResponse>.Failure(
                    ResultError.Unauthorized(
                        "Auth.InvalidCredentials",
                        "Credenciales inválidas."));
            }

            var token = _jwtService.GenerateToken(usuario);
            var expiresAt = _jwtService.GetExpiration();

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
