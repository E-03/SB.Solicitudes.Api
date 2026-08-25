using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.Solicitudes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.ContraseñaHash)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Correo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Rol)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Activo)
                .IsRequired();

            // ====================================================
            // ÍNDICES
            // ====================================================

            builder.HasIndex(x => x.Correo)
                .IsUnique();

            builder.HasIndex(x => x.Rol);

            // ====================================================
            // SOLICITUDES CREADAS
            // ====================================================

            builder.HasMany(x => x.SolicitudesCreadas)
                .WithOne(x => x.UsuarioSolicitante)
                .HasForeignKey(x => x.UsuarioSolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // SOLICITUDES ASIGNADAS
            // ====================================================

            builder.HasMany(x => x.SolicitudesAsignadas)
                .WithOne(x => x.Responsable)
                .HasForeignKey(x => x.ResponsableId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // COMENTARIOS
            // ====================================================

            builder.HasMany(x => x.Comentarios)
                .WithOne(x => x.Usuario)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
