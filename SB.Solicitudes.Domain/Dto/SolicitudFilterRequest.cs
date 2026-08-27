using SB.Solicitudes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Dto
{
    public sealed record SolicitudFilterRequest(
       int? EstadoId,
       EstadoNotificacion? Estado,
       PrioridadesSolicitud? Prioridad,
       int? AreaId,
       int? UsuarioSolicitanteId,
       int? ResponsableId,
       DateTime? FechaDesde,
       DateTime? FechaHasta,
       int PageNumber = 1,
       int PageSize = 10);
}
