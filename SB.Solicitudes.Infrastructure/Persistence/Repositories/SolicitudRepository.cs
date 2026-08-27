using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Common.Pagination;
using SB.Solicitudes.Domain.Dto;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class SolicitudRepository
        : GenericRepository<Solicitud>, ISolicitudRepository
    {
        public SolicitudRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<
            PaginatedResult<SolicitudListItem>>
            GetPagedAsync(
                SolicitudFilterRequest filter,
                CancellationToken cancellationToken)
        {
            var query = DbSet
                .AsNoTracking()
                .AsQueryable();

            if (filter.Estado.HasValue)
            {
                query = query.Where(
                    x => x.Estado == (EstadosSolicitud)filter.Estado.Value);
            }

            if (filter.Prioridad.HasValue)
            {
                query = query.Where(
                    x => x.Prioridad == filter.Prioridad.Value);
            }

            if (filter.AreaId.HasValue)
            {
                query = query.Where(
                    x => x.AreaId == filter.AreaId.Value);
            }

            if (filter.UsuarioSolicitanteId.HasValue)
            {
                query = query.Where(
                    x => x.UsuarioSolicitanteId ==
                         filter.UsuarioSolicitanteId.Value);
            }

            if (filter.ResponsableId.HasValue)
            {
                query = query.Where(
                    x => x.ResponsableId ==
                         filter.ResponsableId.Value);
            }

            if (filter.FechaDesde.HasValue)
            {
                query = query.Where(
                    x => x.FechaCreacion >=
                         filter.FechaDesde.Value);
            }

            if (filter.FechaHasta.HasValue)
            {
                var fechaHasta =
                    filter.FechaHasta.Value.Date
                        .AddDays(1);

                query = query.Where(
                    x => x.FechaCreacion < fechaHasta);
            }

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            var pageNumber =
                filter.PageNumber < 1
                    ? 1
                    : filter.PageNumber;

            var pageSize =
                filter.PageSize switch
                {
                    < 1 => 10,
                    > 100 => 100,
                    _ => filter.PageSize
                };

            var items =
                await query
                    .OrderByDescending(
                        x => x.FechaCreacion)
                    .Skip(
                        (pageNumber - 1) *
                        pageSize)
                    .Take(pageSize)
                    .Select(p => new SolicitudListItem(
                        p.Id,
                        p.Codigo,
                        p.Titulo,
                        p.Prioridad!.Value.ToString(),
                        p.Estado!.Value.ToString(),
                        p.Area.Nombre,
                        p.TipoSolicitud.Nombre,
                        p.UsuarioSolicitante.Nombre,
                        p.Responsable != null
                            ? p.Responsable.Nombre
                            : null,
                        p.FechaCreacion,
                        p.FechaCompromiso))
                    .ToListAsync(
                        cancellationToken);

            return new PaginatedResult<SolicitudListItem>(
                items,
                pageNumber,
                pageSize,
                totalCount);
        }

        public Task<Solicitud?> GetDetailAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return DbSet
                .Include(x => x.Area)
                .Include(x => x.TipoSolicitud)
                .Include(x => x.UsuarioSolicitante)
                .Include(x => x.Responsable)
                //.Include(x => x.Comentarios)
                //.Include(x => x.HistorialEstados)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<SolicitudResponse> CrearSolicitudAsync(CrearSolicitudRequest request, int usuarioSolicitanteId)
        {
            throw new NotImplementedException();
        }
    }
}
