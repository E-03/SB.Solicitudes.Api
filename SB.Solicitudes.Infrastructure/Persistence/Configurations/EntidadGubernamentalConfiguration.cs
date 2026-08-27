using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class EntidadGubernamentalConfiguration : IEntityTypeConfiguration<EntidadGubernamental>
    {
        public void Configure(EntityTypeBuilder<EntidadGubernamental> builder)
        {
            builder.ToTable("EntidadesGubernamentales");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Categoria)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.PoderDelEstado)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Sector)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Nombre);
        }
    }
}
