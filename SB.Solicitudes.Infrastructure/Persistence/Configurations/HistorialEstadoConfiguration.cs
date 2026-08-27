using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class HistorialEstadoConfiguration : IEntityTypeConfiguration<HistorialEstado>
    {
        public void Configure(EntityTypeBuilder<HistorialEstado> builder)
        {
            builder.ToTable("HistorialEstados");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EstadoAnterior)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.EstadoNuevo)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Fecha)
                .IsRequired();

            builder.Property(x => x.Comentario)
                .IsRequired()
                .HasMaxLength(2000);

            // Historial -> Solicitud
            builder.HasOne(x => x.Solicitud)
                .WithMany(x => x.HistorialEstados)
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            // Historial -> Usuario
            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SolicitudId);
            builder.HasIndex(x => x.UsuarioId);
            builder.HasIndex(x => x.Fecha);
        }
    }
}
