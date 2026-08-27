using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Common.Models;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Repositories
{
    public class SolicitudRepository
        : GenericRepository<Solicitud>, ISolicitudRepository
    {
        private static readonly EstadosSolicitud[] EstadosNoAbiertos =
        {
            EstadosSolicitud.Resuelta,
            EstadosSolicitud.Cerrada
        };

        public SolicitudRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<PaginatedResult<SolicitudListItem>> GetPagedAsync(
            SolicitudFilterRequest filter,
            int currentUserId,
            string currentUserRole,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet
                .AsNoTracking()
                .AsQueryable();

            query = AplicarVisibilidad(query, currentUserId, currentUserRole);

            if (filter.Estado.HasValue)
            {
                query = query.Where(x => x.Estado == filter.Estado.Value);
            }

            if (filter.Prioridad.HasValue)
            {
                query = query.Where(x => x.Prioridad == filter.Prioridad.Value);
            }

            if (filter.AreaId.HasValue)
            {
                query = query.Where(x => x.AreaId == filter.AreaId.Value);
            }

            if (currentUserRole == Roles.Administrador &&
                filter.UsuarioSolicitanteId.HasValue)
            {
                query = query.Where(
                    x => x.UsuarioSolicitanteId == filter.UsuarioSolicitanteId.Value);
            }

            if (currentUserRole != Roles.Solicitante &&
                filter.ResponsableId.HasValue)
            {
                query = query.Where(
                    x => x.ResponsableId == filter.ResponsableId.Value);
            }

            if (filter.FechaDesde.HasValue)
            {
                query = query.Where(x => x.FechaCreacion >= filter.FechaDesde.Value);
            }

            if (filter.FechaHasta.HasValue)
            {
                var fechaHasta = filter.FechaHasta.Value.Date.AddDays(1);

                query = query.Where(x => x.FechaCreacion < fechaHasta);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var pageNumber = filter.PageNumber < 1
                ? 1
                : filter.PageNumber;

            var pageSize = filter.PageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => filter.PageSize
            };

            var items = await query
                .OrderByDescending(x => x.FechaCreacion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new SolicitudListItem(
                    p.Id,
                    p.Codigo,
                    p.Titulo,
                    p.Prioridad.ToString(),
                    p.Estado.ToString(),
                    p.Area.Nombre,
                    p.TipoSolicitud.Nombre,
                    p.UsuarioSolicitante.Nombre,
                    p.Responsable != null ? p.Responsable.Nombre : null,
                    p.FechaCreacion,
                    p.FechaCompromiso))
                .ToListAsync(cancellationToken);

            return new PaginatedResult<SolicitudListItem>(
                items,
                pageNumber,
                pageSize,
                totalCount);
        }

        public Task<Solicitud?> GetDetailAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return DbSet
                .Include(x => x.Area)
                .Include(x => x.TipoSolicitud)
                .Include(x => x.UsuarioSolicitante)
                .Include(x => x.Responsable)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<string?> ObtenerUltimoCodigoAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => x.Codigo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<int> CountAbiertasAsync(
            CancellationToken cancellationToken = default)
        {
            return DbSet.CountAsync(
                x => !EstadosNoAbiertos.Contains(x.Estado),
                cancellationToken);
        }

        public Task<int> CountCerradasAsync(
            CancellationToken cancellationToken = default)
        {
            return DbSet.CountAsync(
                x => x.Estado == EstadosSolicitud.Cerrada,
                cancellationToken);
        }

        public Task<int> CountVencidasAsync(
            DateTime now,
            CancellationToken cancellationToken = default)
        {
            return DbSet.CountAsync(
                x => x.FechaCompromiso < now &&
                     x.Estado != EstadosSolicitud.Cerrada &&
                     x.Estado != EstadosSolicitud.Resuelta,
                cancellationToken);
        }

        public async Task<IReadOnlyDictionary<string, int>> CountByEstadoGroupedAsync(
            CancellationToken cancellationToken = default)
        {
            var grupos = await DbSet
                .AsNoTracking()
                .GroupBy(x => x.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToListAsync(cancellationToken);

            return grupos.ToDictionary(
                g => g.Estado.ToString(),
                g => g.Cantidad);
        }

        public async Task<IReadOnlyDictionary<string, int>> CountByPrioridadGroupedAsync(
            CancellationToken cancellationToken = default)
        {
            var grupos = await DbSet
                .AsNoTracking()
                .GroupBy(x => x.Prioridad)
                .Select(g => new { Prioridad = g.Key, Cantidad = g.Count() })
                .ToListAsync(cancellationToken);

            return grupos.ToDictionary(
                g => g.Prioridad.ToString(),
                g => g.Cantidad);
        }

        public async Task<IReadOnlyCollection<Solicitud>> GetUltimasAsync(
            int cantidad,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(x => x.Area)
                .Include(x => x.TipoSolicitud)
                .Include(x => x.UsuarioSolicitante)
                .Include(x => x.Responsable)
                .OrderByDescending(x => x.FechaCreacion)
                .Take(cantidad)
                .ToListAsync(cancellationToken);
        }

        private static IQueryable<Solicitud> AplicarVisibilidad(
            IQueryable<Solicitud> query,
            int currentUserId,
            string currentUserRole)
        {
            return currentUserRole switch
            {
                Roles.Administrador => query,

                Roles.Analista => query.Where(
                    x => x.ResponsableId == null ||
                         x.ResponsableId == currentUserId),

                _ => query.Where(
                    x => x.UsuarioSolicitanteId == currentUserId)
            };
        }
    }
}
