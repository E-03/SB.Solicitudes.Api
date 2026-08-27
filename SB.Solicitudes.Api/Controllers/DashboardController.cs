using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Api.Extensions;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Analista}")]
    public sealed class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetResumenAsync(cancellationToken);

            return result.ToActionResult();
        }
    }
}
