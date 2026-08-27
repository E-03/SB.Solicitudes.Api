using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface INotificacionService
    {
        Task EnviarNotificacionCreacionSolicitudAsync(
            Solicitud solicitud,
            CancellationToken cancellationToken = default);

        Task EnviarNotificacionAsignacionAsync(
            Solicitud solicitud,
            CancellationToken cancellationToken = default);

        Task EnviarNotificacionCambioEstadoAsync(
            Solicitud solicitud,
            string estadoAnterior,
            string estadoNuevo,
            CancellationToken cancellationToken = default);

        Task EnviarNotificacionCierreAsync(
            Solicitud solicitud,
            CancellationToken cancellationToken = default);
    }
}
