using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Api.Extensions;
using SB.Solicitudes.Application.DTOs.Comentarios;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Api.Controllers
{
    [ApiController]
    [Route("api/solicitudes")]
    [Authorize]
    public sealed class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _solicitudService;

        public SolicitudesController(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] SolicitudFilterRequest filter,
            CancellationToken cancellationToken)
        {
            var result = await _solicitudService.GetPagedAsync(filter, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _solicitudService.GetByIdAsync(id, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CrearSolicitudRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _solicitudService.CreateAsync(request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Value!.Id },
                    result.Value)
                : result.ToActionResult();
        }

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> ChangeState(
            int id,
            CambiarEstadoRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _solicitudService.ChangeStateAsync(id, request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPatch("{id:int}/asignacion")]
        public async Task<IActionResult> Assign(
            int id,
            AsignarSolicitudRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _solicitudService.AssignAsync(id, request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("{id:int}/comentarios")]
        public async Task<IActionResult> AddComment(
            int id,
            CrearComentarioRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _solicitudService.AddCommentAsync(id, request, cancellationToken);

            return result.ToActionResult();
        }
    }
}
