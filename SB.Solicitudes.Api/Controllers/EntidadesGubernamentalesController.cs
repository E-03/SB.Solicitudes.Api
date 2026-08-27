using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Api.Extensions;
using SB.Solicitudes.Application.DTOs.EntidadesGubernamentales;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Api.Controllers
{
    [ApiController]
    [Route("api/entidades-gubernamentales")]
    [Authorize(Roles = Roles.Administrador)]
    public sealed class EntidadesGubernamentalesController : ControllerBase
    {
        private readonly IEntidadGubernamentalService _service;

        public EntidadesGubernamentalesController(IEntidadGubernamentalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] EntidadGubernamentalFilterRequest filter,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetPagedAsync(filter, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            EntidadGubernamentalRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Value!.Id },
                    result.Value)
                : result.ToActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            EntidadGubernamentalRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.UpdateAsync(id, request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(id, cancellationToken);

            return result.ToActionResult();
        }
    }
}
