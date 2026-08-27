using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Features.Notificacion
{
    public interface INotificacionHandler
    {
        Task EnviarAsync(
            Domain.Entities.Notificacion notificacion,
            CancellationToken cancellationToken);
    }

    public sealed class ConsoleNotificacionHandler : INotificacionHandler
    {
        public Task EnviarAsync(
            Domain.Entities.Notificacion notificacion,
            CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[NOTIFICACIÓN - {DateTime.Now:HH:mm:ss}]");
            Console.WriteLine($"Canal: {notificacion.Canal}");
            Console.WriteLine($"Asunto: {notificacion.Asunto}");
            Console.WriteLine($"Mensaje: {notificacion.Mensaje}");
            Console.WriteLine(new string('-', 50));
            Console.ResetColor();

            return Task.CompletedTask;
        }
    }

    public sealed class BaseDatosNotificacionHandler : INotificacionHandler
    {
        private readonly INotificacionRepository _notificacionRepository;

        public BaseDatosNotificacionHandler(
            INotificacionRepository notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        public async Task EnviarAsync(
            Domain.Entities.Notificacion notificacion,
            CancellationToken cancellationToken)
        {
            notificacion.MarcarEnviada();

            await _notificacionRepository.AddAsync(
                notificacion,
                cancellationToken);
        }
    }

    public sealed class NotificacionService : INotificacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReadOnlyCollection<INotificacionHandler> _handlers;

        public NotificacionService(
            IUnitOfWork unitOfWork,
            INotificacionRepository notificacionRepository)
        {
            _unitOfWork = unitOfWork;
            _handlers = new List<INotificacionHandler>
            {
                new ConsoleNotificacionHandler(),
                new BaseDatosNotificacionHandler(notificacionRepository)
            };
        }

        public Task EnviarNotificacionCreacionSolicitudAsync(
            Solicitud solicitud,
            CancellationToken cancellationToken = default)
        {
            var notificacion = new Domain.Entities.Notificacion(
                solicitud.Id,
                solicitud.UsuarioSolicitanteId,
                CanalNotificacion.ConsoleSimulacion,
                "Nueva solicitud creada",
                $"Su solicitud '{solicitud.Titulo}' ha sido registrada exitosamente. Código: {solicitud.Codigo}");

            return EjecutarNotificacionAsync(notificacion, cancellationToken);
        }

        public Task EnviarNotificacionAsignacionAsync(
            Solicitud solicitud,
            CancellationToken cancellationToken = default)
        {
            if (solicitud.ResponsableId is null)
            {
                return Task.CompletedTask;
            }

            var notificacion = new Domain.Entities.Notificacion(
                solicitud.Id,
                solicitud.ResponsableId.Value,
                CanalNotificacion.ConsoleSimulacion,
                "Solicitud asignada a usted",
                $"La solicitud '{solicitud.Titulo}' ha sido asignada a su responsabilidad. Código: {solicitud.Codigo}");

            return EjecutarNotificacionAsync(notificacion, cancellationToken);
        }

        public Task EnviarNotificacionCambioEstadoAsync(
            Solicitud solicitud,
            string estadoAnterior,
            string estadoNuevo,
            CancellationToken cancellationToken = default)
        {
            var notificacion = new Domain.Entities.Notificacion(
                solicitud.Id,
                solicitud.UsuarioSolicitanteId,
                CanalNotificacion.ConsoleSimulacion,
                "Cambio de estado en solicitud",
                $"La solicitud '{solicitud.Titulo}' cambió de estado de {estadoAnterior} a {estadoNuevo}.");

            return EjecutarNotificacionAsync(notificacion, cancellationToken);
        }

        public Task EnviarNotificacionCierreAsync(
            Solicitud solicitud,
            CancellationToken cancellationToken = default)
        {
            var notificacion = new Domain.Entities.Notificacion(
                solicitud.Id,
                solicitud.UsuarioSolicitanteId,
                CanalNotificacion.ConsoleSimulacion,
                "Solicitud cerrada",
                $"La solicitud '{solicitud.Titulo}' ha sido cerrada. Código: {solicitud.Codigo}");

            return EjecutarNotificacionAsync(notificacion, cancellationToken);
        }

        private async Task EjecutarNotificacionAsync(
            Domain.Entities.Notificacion notificacion,
            CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
            {
                try
                {
                    await handler.EnviarAsync(notificacion, cancellationToken);
                }
                catch (Exception)
                {
                    notificacion.MarcarFallida();
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
