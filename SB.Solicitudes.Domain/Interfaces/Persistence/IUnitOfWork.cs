namespace SB.Solicitudes.Domain.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        IUsuarioRepository Usuarios { get; }

        IAreaRepository Areas { get; }

        IComentarioRepository Comentarios { get; }

        IHistorialEstadoRepository HistorialEstados { get; }

        INotificacionRepository Notificaciones { get; }

        ISolicitudRepository Solicitudes { get; }

        ITipoSolicitudRepository TiposSolicitud { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
