using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
    {
        public void Configure(EntityTypeBuilder<Comentario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Texto)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.Visibilidad)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Fecha)
                .IsRequired();

            // Comentario -> Solicitud
            builder.HasOne(x => x.Solicitud)
                .WithMany()
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comentario -> Usuario
            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.Comentarios)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SolicitudId);
            builder.HasIndex(x => x.UsuarioId);
        }
    }

}
