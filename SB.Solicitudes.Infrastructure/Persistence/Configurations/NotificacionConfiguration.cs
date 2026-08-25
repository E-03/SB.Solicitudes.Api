using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
    {
        public void Configure(EntityTypeBuilder<Notificacion> builder)
        {
            builder.ToTable("Notificaciones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Canal)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Asunto)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Mensaje)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.Estado)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Fecha)
                .IsRequired();

            // Notificacion -> Solicitud
            builder.HasOne(x => x.Solicitud)
                .WithMany()
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notificacion -> Usuario destino
            builder.HasOne(x => x.UsuarioDestino)
                .WithMany()
                .HasForeignKey(x => x.UsuarioDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SolicitudId);
            builder.HasIndex(x => x.UsuarioDestinoId);
            builder.HasIndex(x => x.Fecha);
        }
    }
}
