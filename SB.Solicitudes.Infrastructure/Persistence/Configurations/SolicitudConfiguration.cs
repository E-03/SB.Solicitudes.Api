using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class SolicitudConfiguration : IEntityTypeConfiguration<Solicitud>
    {
        public void Configure(EntityTypeBuilder<Solicitud> builder)
        {
            builder.ToTable("Solicitudes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Codigo)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Descripcion)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(x => x.Prioridad)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Estado)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.FechaCreacion)
                .IsRequired();

            builder.Property(x => x.FechaCompromiso)
                .IsRequired();

            // ====================================================
            // SOLICITANTE
            // ====================================================

            builder.HasOne(x => x.UsuarioSolicitante)
                .WithMany(x => x.SolicitudesCreadas)
                .HasForeignKey(x => x.UsuarioSolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // RESPONSABLE
            // ====================================================

            builder.HasOne(x => x.Responsable)
                .WithMany(x => x.SolicitudesAsignadas)
                .HasForeignKey(x => x.ResponsableId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // AREA
            // ====================================================

            builder.HasOne(x => x.Area)
                .WithMany(x => x.Solicitudes)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // TIPO DE SOLICITUD
            // ====================================================

            builder.HasOne(x => x.TipoSolicitud)
                .WithMany(x => x.Solicitudes)
                .HasForeignKey(x => x.TipoSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // INDICES
            // ====================================================

            builder.HasIndex(x => x.Codigo)
                .IsUnique();

            builder.HasIndex(x => x.Estado);

            builder.HasIndex(x => x.Prioridad);

            builder.HasIndex(x => x.UsuarioSolicitanteId);

            builder.HasIndex(x => x.ResponsableId);

            builder.HasIndex(x => x.AreaId);

            builder.HasIndex(x => x.TipoSolicitudId);

            builder.HasIndex(x => x.FechaCreacion);

            builder.HasIndex(x => x.FechaCompromiso);
        }
    }
}
