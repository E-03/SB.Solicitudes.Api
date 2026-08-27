using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Dto
{
    public class SolicitudResponse
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? UrlEvidencia { get; set; }
        public string? ReferenciaEvidencia { get; set; }
        public int UsuarioSolicitanteId { get; set; }
        public string UsuarioSolicitanteNombre { get; set; }
        public int? ResponsableId { get; set; }
        public string? ResponsableNombre { get; set; }
        public int AreaId { get; set; }
        public string AreaNombre { get; set; }
        public int TipoSolicitudId { get; set; }
        public string TipoSolicitudNombre { get; set; }
    }
}
