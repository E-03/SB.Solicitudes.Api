using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Domain.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Features.Notificacion
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly List<INotificacionHandler> _handlers;

        public NotificacionService(
            INotificacionRepository notificacionRepository,
            IUnitOfWork unitOfWork)
        {
            _notificacionRepository = notificacionRepository;
            _unitOfWork = unitOfWork;
            _handlers = new List<INotificacionHandler>()
            {
                new ConsoleNotificacionHandler(),
                new BaseDatosNotificacionHandler(notificacionRepository)
            };
        }

        public async Task EnviarNotificacionCreacionSolicitudAsync(Solicitud solicitud)
        {
            var notificacion = new SB.Solicitudes.Domain.Entities.Notificacion
            {
                SolicitudId = solicitud.Id,
                UsuarioDestinoId = solicitud.UsuarioSolicitanteId,
                Canal = CanalNotificacion.ConsoleSimulacion,
                Asunto = "Nueva solicitud creada",
                Mensaje = $"Su solicitud '{solicitud.Titulo}' ha sido registrada exitosamente. Código: {solicitud.Codigo}",
                Estado = EstadoNotificacion.Pendiente,
                Fecha = DateTime.UtcNow
            };

            await EjecutarNotificacionAsync(notificacion);
        }

        public async Task EnviarNotificacionAsignacionAsync(Solicitud solicitud)
        {
            if (solicitud.ResponsableId == null)
                return;

            var notificacion = new SB.Solicitudes.Domain.Entities.Notificacion
            {
                SolicitudId = solicitud.Id,
                UsuarioDestinoId = solicitud.ResponsableId.Value,
                Canal = CanalNotificacion.ConsoleSimulacion,
                Asunto = "Solicitud asignada a usted",
                Mensaje = $"La solicitud '{solicitud.Titulo}' ha sido asignada a su responsabilidad. Código: {solicitud.Codigo}",
                Estado = SB.Solicitudes.Domain.Common.EstadoNotificacion.Pendiente,
                Fecha = DateTime.UtcNow
            };

            await EjecutarNotificacionAsync(notificacion);
        }

        public async Task EnviarNotificacionCambioEstadoAsync(Solicitud solicitud, string estadoAnterior, string estadoNuevo)
        {
            var notificacion = new SB.Solicitudes.Domain.Entities.Notificacion
            {
                SolicitudId = solicitud.Id,
                UsuarioDestinoId = solicitud.UsuarioSolicitanteId,
                Canal = CanalNotificacion.ConsoleSimulacion,
                Asunto = "Cambio de estado en solicitud",
                Mensaje = $"La solicitud '{solicitud.Titulo}' ha cambiado de estado de {estadoAnterior} a {estadoNuevo}",
                Estado = EstadoNotificacion.Pendiente,
                Fecha = DateTime.UtcNow
            };

            await EjecutarNotificacionAsync(notificacion);
        }

        public async Task EnviarNotificacionCierreAsync(Solicitud solicitud)
        {
            var notificacion = new SB.Solicitudes.Domain.Entities.Notificacion
            {
                SolicitudId = solicitud.Id,
                UsuarioDestinoId = solicitud.UsuarioSolicitanteId,
                Canal = CanalNotificacion.ConsoleSimulacion,
                Asunto = "Solicitud cerrada",
                Mensaje = $"La solicitud '{solicitud.Titulo}' ha sido cerrada. Código: {solicitud.Codigo}",
                Estado = EstadoNotificacion.Pendiente,
                Fecha = DateTime.UtcNow
            };

            await EjecutarNotificacionAsync(notificacion);
        }

        private async Task EjecutarNotificacionAsync(SB.Solicitudes.Domain.Entities.Notificacion notificacion)
        {
            foreach (var handler in _handlers)
            {
                try
                {
                    await handler.EnviarAsync(notificacion);
                }
                catch (Exception ex)
                {
                    notificacion.Estado = SB.Solicitudes.Domain.Common.EstadoNotificacion.Fallida;
                    Console.WriteLine($"Error en handler de notificación: {ex.Message}");
                }
            }
        }
    }

    public interface INotificacionHandler
    {
        Task EnviarAsync(SB.Solicitudes.Domain.Entities.Notificacion notificacion);
    }

    public class ConsoleNotificacionHandler : INotificacionHandler
    {
        public Task EnviarAsync(SB.Solicitudes.Domain.Entities.Notificacion notificacion)
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

    public class BaseDatosNotificacionHandler : INotificacionHandler
    {
        private readonly INotificacionRepository _notificacionRepository;

        public BaseDatosNotificacionHandler(INotificacionRepository notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        public async Task EnviarAsync(SB.Solicitudes.Domain.Entities.Notificacion notificacion)
        {
            notificacion.Estado = SB.Solicitudes.Domain.Common.EstadoNotificacion.Enviada;
            await _notificacionRepository.AddAsync(notificacion);
            await Task.CompletedTask;
        }
    }
}
