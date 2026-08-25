using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

        public DbSet<Area> Areas => Set<Area>();

        public DbSet<TipoSolicitud> TiposSolicitud => Set<TipoSolicitud>();

        public DbSet<Comentario> Comentarios => Set<Comentario>();

        public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();

        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly
            );
        }
    }
}