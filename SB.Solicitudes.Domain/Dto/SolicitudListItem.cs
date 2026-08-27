using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Dto
{
    public sealed record SolicitudListItem(
        int Id,
        string Codigo,
        string Titulo,
        string Prioridad,
        string Estado,
        string Area,
        string TipoSolicitud,
        string UsuarioSolicitante,
        string? Responsable,
        DateTime FechaCreacion,
        DateTime FechaCompromiso);
}
