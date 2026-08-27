using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.EntidadesGubernamentales;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class EntidadGubernamentalRepository
        : GenericRepository<EntidadGubernamental>, IEntidadGubernamentalRepository
    {
        public EntidadGubernamentalRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<PaginatedResult<EntidadGubernamentalResponse>> GetPagedAsync(
            EntidadGubernamentalFilterRequest filter,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    x.Nombre.Contains(search) ||
                    x.Categoria.Contains(search) ||
                    x.Sector.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;

            var pageSize = filter.PageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => filter.PageSize
            };

            var items = await query
                .OrderBy(x => x.Nombre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EntidadGubernamentalResponse(
                    x.Id,
                    x.Nombre,
                    x.Categoria,
                    x.PoderDelEstado,
                    x.Sector))
                .ToListAsync(cancellationToken);

            return new PaginatedResult<EntidadGubernamentalResponse>(
                items,
                pageNumber,
                pageSize,
                totalCount);
        }
    }
}
