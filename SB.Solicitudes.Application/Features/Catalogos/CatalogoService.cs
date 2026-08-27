using SB.Solicitudes.Application.DTOs.Catalogos;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Application.Features.Catalogos
{
    public sealed class CatalogoService : ICatalogoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyCollection<AreaResponse>> GetAreasActivasAsync(
            CancellationToken cancellationToken)
        {
            var areas = await _unitOfWork.Areas.GetActivasAsync(cancellationToken);

            return areas
                .Select(a => new AreaResponse(a.Id, a.Nombre))
                .ToList();
        }

        public async Task<IReadOnlyCollection<TipoSolicitudResponse>> GetTiposActivosAsync(
            CancellationToken cancellationToken)
        {
            var tipos = await _unitOfWork.TiposSolicitud.GetActivosAsync(cancellationToken);

            return tipos
                .Select(t => new TipoSolicitudResponse(t.Id, t.Nombre, t.Descripcion))
                .ToList();
        }
    }
}
