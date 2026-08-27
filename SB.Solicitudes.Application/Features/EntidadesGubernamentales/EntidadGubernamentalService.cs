using FluentValidation;
using SB.Solicitudes.Application.Common.Extensions;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.EntidadesGubernamentales;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Application.Features.EntidadesGubernamentales
{
    public sealed class EntidadGubernamentalService : IEntidadGubernamentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<EntidadGubernamentalRequest> _validator;

        public EntidadGubernamentalService(
            IUnitOfWork unitOfWork,
            IValidator<EntidadGubernamentalRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<ResultEntity<PaginatedResult<EntidadGubernamentalResponse>>> GetPagedAsync(
            EntidadGubernamentalFilterRequest filter,
            CancellationToken cancellationToken)
        {
            var paged = await _unitOfWork.EntidadesGubernamentales
                .GetPagedAsync(filter, cancellationToken);

            return ResultEntity<PaginatedResult<EntidadGubernamentalResponse>>.Success(paged);
        }

        public async Task<ResultEntity<EntidadGubernamentalResponse>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var entidad = await _unitOfWork.EntidadesGubernamentales
                .GetByIdAsync(id, cancellationToken);

            if (entidad is null)
            {
                return NotFound();
            }

            return ResultEntity<EntidadGubernamentalResponse>.Success(Map(entidad));
        }

        public async Task<ResultEntity<EntidadGubernamentalResponse>> CreateAsync(
            EntidadGubernamentalRequest request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return ResultEntity<EntidadGubernamentalResponse>.Failure(
                    validation.ToResultErrors());
            }

            var entidad = new EntidadGubernamental(
                request.Nombre,
                request.Categoria,
                request.PoderDelEstado,
                request.Sector);

            await _unitOfWork.EntidadesGubernamentales.AddAsync(entidad, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultEntity<EntidadGubernamentalResponse>.Success(Map(entidad));
        }

        public async Task<ResultEntity<EntidadGubernamentalResponse>> UpdateAsync(
            int id,
            EntidadGubernamentalRequest request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return ResultEntity<EntidadGubernamentalResponse>.Failure(
                    validation.ToResultErrors());
            }

            var entidad = await _unitOfWork.EntidadesGubernamentales
                .GetByIdAsync(id, cancellationToken);

            if (entidad is null)
            {
                return NotFound();
            }

            entidad.Actualizar(
                request.Nombre,
                request.Categoria,
                request.PoderDelEstado,
                request.Sector);

            _unitOfWork.EntidadesGubernamentales.Update(entidad);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultEntity<EntidadGubernamentalResponse>.Success(Map(entidad));
        }

        public async Task<Result> DeleteAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var entidad = await _unitOfWork.EntidadesGubernamentales
                .GetByIdAsync(id, cancellationToken);

            if (entidad is null)
            {
                return Result.Failure(
                    ResultError.NotFound(
                        "EntidadGubernamental.NotFound",
                        "La entidad gubernamental no existe."));
            }

            _unitOfWork.EntidadesGubernamentales.Remove(entidad);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private static ResultEntity<EntidadGubernamentalResponse> NotFound()
        {
            return ResultEntity<EntidadGubernamentalResponse>.Failure(
                ResultError.NotFound(
                    "EntidadGubernamental.NotFound",
                    "La entidad gubernamental no existe."));
        }

        private static EntidadGubernamentalResponse Map(EntidadGubernamental entidad)
        {
            return new EntidadGubernamentalResponse(
                entidad.Id,
                entidad.Nombre,
                entidad.Categoria,
                entidad.PoderDelEstado,
                entidad.Sector);
        }
    }
}
