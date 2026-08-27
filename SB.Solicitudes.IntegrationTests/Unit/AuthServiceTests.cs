using Moq;
using SB.Solicitudes.Application.DTOs.Auth;
using SB.Solicitudes.Application.Features.Auth;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Application.Validators;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;
using Xunit;

namespace SB.Solicitudes.IntegrationTests.Unit
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<IJwtService> _jwtService = new();
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _unitOfWork.Setup(u => u.Usuarios).Returns(_usuarioRepository.Object);

            _sut = new AuthService(
                _unitOfWork.Object,
                _passwordHasher.Object,
                _jwtService.Object,
                new LoginRequestValidator());
        }

        [Fact]
        public async Task LoginAsync_UsuarioInactivo_DevuelveUnauthorized()
        {
            var usuario = new Usuario("Demo", "demo@test.local", "hash", Roles.Solicitante);
            usuario.Desactivar();

            _usuarioRepository
                .Setup(r => r.GetByCorreoAsync("demo@test.local", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var result = await _sut.LoginAsync(
                new LoginRequest("demo@test.local", "Password123!"),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(Application.Common.Models.ErrorType.Unauthorized, result.Errors.First().Type);
        }

        [Fact]
        public async Task LoginAsync_PasswordIncorrecto_DevuelveUnauthorized()
        {
            var usuario = new Usuario("Demo", "demo@test.local", "hash", Roles.Solicitante);

            _usuarioRepository
                .Setup(r => r.GetByCorreoAsync("demo@test.local", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _passwordHasher
                .Setup(h => h.Verify("wrong", "hash"))
                .Returns(false);

            var result = await _sut.LoginAsync(
                new LoginRequest("demo@test.local", "wrong"),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(Application.Common.Models.ErrorType.Unauthorized, result.Errors.First().Type);
        }

        [Fact]
        public async Task LoginAsync_CredencialesValidas_DevuelveToken()
        {
            var usuario = new Usuario("Demo", "demo@test.local", "hash", Roles.Administrador);

            _usuarioRepository
                .Setup(r => r.GetByCorreoAsync("demo@test.local", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _passwordHasher
                .Setup(h => h.Verify("Password123!", "hash"))
                .Returns(true);

            _jwtService
                .Setup(j => j.GenerateToken(usuario))
                .Returns("fake-token");

            _jwtService
                .Setup(j => j.GetExpiration())
                .Returns(DateTime.UtcNow.AddHours(1));

            var result = await _sut.LoginAsync(
                new LoginRequest("demo@test.local", "Password123!"),
                CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("fake-token", result.Value!.Token);
        }
    }
}
