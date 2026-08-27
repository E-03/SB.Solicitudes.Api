using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Api.Controllers
{
    [ApiController]
    [Route("api/catalogos")]
    [Authorize]
    public sealed class CatalogosController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogosController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet("areas")]
        public async Task<IActionResult> GetAreas(CancellationToken cancellationToken)
        {
            var areas = await _catalogoService.GetAreasActivasAsync(cancellationToken);

            return Ok(areas);
        }

        [HttpGet("tipos-solicitud")]
        public async Task<IActionResult> GetTipos(CancellationToken cancellationToken)
        {
            var tipos = await _catalogoService.GetTiposActivosAsync(cancellationToken);

            return Ok(tipos);
        }
    }
}
