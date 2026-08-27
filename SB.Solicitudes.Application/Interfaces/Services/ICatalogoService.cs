using SB.Solicitudes.Application.DTOs.Catalogos;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface ICatalogoService
    {
        Task<IReadOnlyCollection<AreaResponse>> GetAreasActivasAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<TipoSolicitudResponse>> GetTiposActivosAsync(
            CancellationToken cancellationToken);
    }
}
