using Moq;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Comentarios;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Application.Features.Solicitudes;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Application.Validators;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;
using Xunit;

namespace SB.Solicitudes.IntegrationTests.Unit
{
    public class SolicitudServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<ISolicitudRepository> _solicitudRepository = new();
        private readonly Mock<IComentarioRepository> _comentarioRepository = new();
        private readonly Mock<IHistorialEstadoRepository> _historialRepository = new();
        private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
        private readonly Mock<ICurrentUserService> _currentUser = new();
        private readonly Mock<INotificacionService> _notificacionService = new();

        private SolicitudService BuildService()
        {
            _unitOfWork.Setup(u => u.Solicitudes).Returns(_solicitudRepository.Object);
            _unitOfWork.Setup(u => u.Comentarios).Returns(_comentarioRepository.Object);
            _unitOfWork.Setup(u => u.HistorialEstados).Returns(_historialRepository.Object);
            _unitOfWork.Setup(u => u.Usuarios).Returns(_usuarioRepository.Object);

            return new SolicitudService(
                _unitOfWork.Object,
                _currentUser.Object,
                _notificacionService.Object,
                new CrearSolicitudRequestValidator(),
                new CambiarEstadoRequestValidator(),
                new AsignarSolicitudRequestValidator(),
                new CrearComentarioRequestValidator());
        }

        private void SetCurrentUser(int userId, string role)
        {
            _currentUser.Setup(c => c.IsAuthenticated).Returns(true);
            _currentUser.Setup(c => c.UserId).Returns(userId);
            _currentUser.Setup(c => c.Role).Returns(role);
            _currentUser.Setup(c => c.IsInRole(It.IsAny<string>()))
                .Returns((string r) => r == role);
        }

        private static Solicitud CrearSolicitud(int solicitanteId = 3)
        {
            return new Solicitud(
                "SOL-2026-0001",
                "Titulo",
                "Descripcion",
                PrioridadesSolicitud.Alta,
                DateTime.UtcNow.AddDays(3),
                solicitanteId,
                areaId: 1,
                tipoSolicitudId: 1,
                urlEvidencia: null,
                referenciaEvidencia: null);
        }

        [Fact]
        public async Task ChangeStateAsync_SinComentario_DevuelveErrorDeValidacion()
        {
            SetCurrentUser(1, Roles.Administrador);

            var solicitud = CrearSolicitud();
            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var sut = BuildService();

            var result = await sut.ChangeStateAsync(
                1,
                new CambiarEstadoRequest("EnAnalisis", ""),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Validation, result.Errors.First().Type);
        }

        [Fact]
        public async Task ChangeStateAsync_CerrarSinComentarioDeResolucion_DevuelveError()
        {
            SetCurrentUser(1, Roles.Administrador);

            var solicitud = CrearSolicitud();
            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            _comentarioRepository
                .Setup(r => r.ExisteComentarioResolucionAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var sut = BuildService();

            var result = await sut.ChangeStateAsync(
                1,
                new CambiarEstadoRequest("Cerrada", "Se cierra"),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Contains(result.Errors, e => e.Code == "Solicitud.ResolutionRequired");
        }

        [Fact]
        public async Task ChangeStateAsync_CerrarConComentarioDeResolucion_RegistraHistorialYNotifica()
        {
            SetCurrentUser(1, Roles.Administrador);

            var solicitud = CrearSolicitud();
            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            _comentarioRepository
                .Setup(r => r.ExisteComentarioResolucionAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var sut = BuildService();

            var result = await sut.ChangeStateAsync(
                1,
                new CambiarEstadoRequest("Cerrada", "Se cierra con resolución"),
                CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(EstadosSolicitud.Cerrada, solicitud.Estado);

            _historialRepository.Verify(
                r => r.AddAsync(
                    It.Is<HistorialEstado>(h =>
                        h.EstadoAnterior == EstadosSolicitud.Registrada &&
                        h.EstadoNuevo == EstadosSolicitud.Cerrada),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _notificacionService.Verify(
                n => n.EnviarNotificacionCierreAsync(solicitud, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ChangeStateAsync_ReabrirComoSolicitante_DevuelveForbidden()
        {
            SetCurrentUser(3, Roles.Solicitante);

            var solicitud = CrearSolicitud(solicitanteId: 3);
            solicitud.CambiarEstado(EstadosSolicitud.Cerrada);

            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var sut = BuildService();

            var result = await sut.ChangeStateAsync(
                1,
                new CambiarEstadoRequest("EnAnalisis", "Quiero reabrir"),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Forbidden, result.Errors.First().Type);
            Assert.Equal(EstadosSolicitud.Cerrada, solicitud.Estado);
        }

        [Fact]
        public async Task ChangeStateAsync_ReabrirComoAnalista_Exitoso()
        {
            SetCurrentUser(2, Roles.Analista);

            var solicitud = CrearSolicitud(solicitanteId: 3);
            solicitud.AsignarResponsable(2);
            solicitud.CambiarEstado(EstadosSolicitud.Cerrada);

            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var sut = BuildService();

            var result = await sut.ChangeStateAsync(
                1,
                new CambiarEstadoRequest("EnAnalisis", "Se reabre para revisión"),
                CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(EstadosSolicitud.EnAnalisis, solicitud.Estado);
        }

        [Fact]
        public async Task AssignAsync_ComoSolicitante_DevuelveForbidden()
        {
            SetCurrentUser(3, Roles.Solicitante);

            var sut = BuildService();

            var result = await sut.AssignAsync(
                1,
                new AsignarSolicitudRequest(2),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Forbidden, result.Errors.First().Type);
        }

        [Fact]
        public async Task AssignAsync_ResponsableNoEsAnalistaActivo_DevuelveValidationError()
        {
            SetCurrentUser(1, Roles.Administrador);

            var solicitud = CrearSolicitud();
            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var solicitanteComoResponsable = new Usuario(
                "Solicitante", "sol@test.local", "hash", Roles.Solicitante);

            _usuarioRepository
                .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitanteComoResponsable);

            var sut = BuildService();

            var result = await sut.AssignAsync(
                1,
                new AsignarSolicitudRequest(5),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Validation, result.Errors.First().Type);
        }

        [Fact]
        public async Task AssignAsync_ResponsableAnalistaActivo_AsignaYNotifica()
        {
            SetCurrentUser(1, Roles.Administrador);

            var solicitud = CrearSolicitud();
            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var analista = new Usuario(
                "Analista", "analista@test.local", "hash", Roles.Analista);

            _usuarioRepository
                .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(analista);

            var sut = BuildService();

            var result = await sut.AssignAsync(
                1,
                new AsignarSolicitudRequest(2),
                CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(analista.Id, solicitud.ResponsableId);

            _notificacionService.Verify(
                n => n.EnviarNotificacionAsignacionAsync(solicitud, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddCommentAsync_ComentarioInternoComoSolicitante_DevuelveForbidden()
        {
            SetCurrentUser(3, Roles.Solicitante);

            var solicitud = CrearSolicitud(solicitanteId: 3);
            _solicitudRepository
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(solicitud);

            var sut = BuildService();

            var result = await sut.AddCommentAsync(
                1,
                new CrearComentarioRequest("Comentario interno", "Interno"),
                CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Forbidden, result.Errors.First().Type);
        }
    }
}
