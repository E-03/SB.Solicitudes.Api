using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class TipoSolicitudConfiguration : IEntityTypeConfiguration<TipoSolicitud>
    {
        public void Configure(EntityTypeBuilder<TipoSolicitud> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.Property(x => x.Activo)
                .IsRequired();

            builder.HasMany(x => x.Solicitudes)
                .WithOne(x => x.TipoSolicitud)
                .HasForeignKey(x => x.TipoSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Nombre)
                .IsUnique();
        }
    }




}
