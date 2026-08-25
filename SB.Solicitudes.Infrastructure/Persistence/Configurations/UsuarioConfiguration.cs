using SB.Solicitudes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Infrastructure.Persistence.Configurations
{
    //public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    //{
    //    public void Configure(EntityTypeBuilder<Usuario> builder)
    //    {
    //        builder.HasKey(u => u.Id);

    //        builder.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
    //        builder.Property(u => u.Correo).IsRequired().HasMaxLength(150);
    //        builder.Property(u => u.Rol).IsRequired().HasMaxLength(50);

    //        builder.HasMany(u => u.SolicitudesCreadas)
    //               .WithOne(s => s.UsuarioSolicitante)
    //               .HasForeignKey(s => s.UsuarioSolicitanteId);

    //        builder.HasMany(u => u.SolicitudesAsignadas)
    //               .WithOne(s => s.Responsable)
    //               .HasForeignKey(s => s.ResponsableId);

    //        builder.HasMany(u => u.Comentarios)
    //               .WithOne(c => c.Usuario)
    //               .HasForeignKey(c => c.UsuarioId);
    //    }
    //}
}
