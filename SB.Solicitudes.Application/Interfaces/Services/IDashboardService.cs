using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Dashboard;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<ResultEntity<DashboardResumenDto>> GetResumenAsync(
            CancellationToken cancellationToken);
    }
}
