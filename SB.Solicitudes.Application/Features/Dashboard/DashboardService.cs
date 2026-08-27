using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Dashboard;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Application.Features.Dashboard
{
    public sealed class DashboardService : IDashboardService
    {
        private const int CantidadUltimas = 5;

        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultEntity<DashboardResumenDto>> GetResumenAsync(
            CancellationToken cancellationToken)
        {
            var abiertas = await _unitOfWork.Solicitudes
                .CountAbiertasAsync(cancellationToken);

            var cerradas = await _unitOfWork.Solicitudes
                .CountCerradasAsync(cancellationToken);

            var vencidas = await _unitOfWork.Solicitudes
                .CountVencidasAsync(DateTime.UtcNow, cancellationToken);

            var porEstado = await _unitOfWork.Solicitudes
                .CountByEstadoGroupedAsync(cancellationToken);

            var porPrioridad = await _unitOfWork.Solicitudes
                .CountByPrioridadGroupedAsync(cancellationToken);

            var ultimas = await _unitOfWork.Solicitudes
                .GetUltimasAsync(CantidadUltimas, cancellationToken);

            var resumen = new DashboardResumenDto(
                abiertas,
                cerradas,
                vencidas,
                porEstado,
                porPrioridad,
                ultimas
                    .Select(s => new SolicitudListItem(
                        s.Id,
                        s.Codigo,
                        s.Titulo,
                        s.Prioridad.ToString(),
                        s.Estado.ToString(),
                        s.Area.Nombre,
                        s.TipoSolicitud.Nombre,
                        s.UsuarioSolicitante.Nombre,
                        s.Responsable?.Nombre,
                        s.FechaCreacion,
                        s.FechaCompromiso))
                    .ToList());

            return ResultEntity<DashboardResumenDto>.Success(resumen);
        }
    }
}
