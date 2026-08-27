using SB.Solicitudes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface INotificacionService
    {
        Task EnviarNotificacionCreacionSolicitudAsync(Solicitud solicitud);
        Task EnviarNotificacionAsignacionAsync(Solicitud solicitud);
        Task EnviarNotificacionCambioEstadoAsync(Solicitud solicitud, string estadoAnterior, string estadoNuevo);
        Task EnviarNotificacionCierreAsync(Solicitud solicitud);
    }
}
