using SB.Solicitudes.Domain.Interfaces.Persistence;

namespace SB.Solicitudes.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUsuarioRepository Usuarios { get; }

        public IAreaRepository Areas { get; }

        public IComentarioRepository Comentarios { get; }

        public IHistorialEstadoRepository HistorialEstados { get; }

        public INotificacionRepository Notificaciones { get; }

        public ISolicitudRepository Solicitudes { get; }

        public ITipoSolicitudRepository TiposSolicitud { get; }

        public UnitOfWork(
            AppDbContext context,
            IUsuarioRepository usuarios,
            IAreaRepository areas,
            IComentarioRepository comentarios,
            IHistorialEstadoRepository historialEstados,
            INotificacionRepository notificaciones,
            ISolicitudRepository solicitudes,
            ITipoSolicitudRepository tiposSolicitud)
        {
            _context = context;

            Usuarios = usuarios;
            Areas = areas;
            Comentarios = comentarios;
            HistorialEstados = historialEstados;
            Notificaciones = notificaciones;
            Solicitudes = solicitudes;
            TiposSolicitud = tiposSolicitud;
        }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(
                cancellationToken);
        }
    } 
}
