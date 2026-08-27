using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.EntidadesGubernamentales;

namespace SB.Solicitudes.Application.Interfaces.Services
{
    public interface IEntidadGubernamentalService
    {
        Task<ResultEntity<PaginatedResult<EntidadGubernamentalResponse>>> GetPagedAsync(
            EntidadGubernamentalFilterRequest filter,
            CancellationToken cancellationToken);

        Task<ResultEntity<EntidadGubernamentalResponse>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken);

        Task<ResultEntity<EntidadGubernamentalResponse>> CreateAsync(
            EntidadGubernamentalRequest request,
            CancellationToken cancellationToken);

        Task<ResultEntity<EntidadGubernamentalResponse>> UpdateAsync(
            int id,
            EntidadGubernamentalRequest request,
            CancellationToken cancellationToken);

        Task<Result> DeleteAsync(
            int id,
            CancellationToken cancellationToken);
    }
}
