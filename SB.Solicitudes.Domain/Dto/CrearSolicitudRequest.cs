using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Dto
{
    public record CrearSolicitudRequest(
        string Titulo,
        string Descripcion,
        string Prioridad,
        int AreaId,
        int TipoSolicitudId,
        DateTime FechaCompromiso,
        string? UrlEvidencia,
        string? ReferenciaEvidencia
    );
}
