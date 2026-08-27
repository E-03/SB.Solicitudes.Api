using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            AppDbContext context,
            IPasswordHasher passwordHasher)
        {
            await context.Database.MigrateAsync();

            if (!await context.Areas.AnyAsync())
            {
                await context.Areas.AddRangeAsync(
                    new Area("Tecnología"),
                    new Area("Canales"),
                    new Area("Operaciones"),
                    new Area("Seguridad"));
            }

            if (!await context.TiposSolicitud.AnyAsync())
            {
                await context.TiposSolicitud.AddRangeAsync(
                    new TipoSolicitud("Incidente", "Falla o interrupción de un servicio existente."),
                    new TipoSolicitud("Requerimiento", "Solicitud de un nuevo servicio o funcionalidad."),
                    new TipoSolicitud("Acceso", "Solicitud de acceso a sistemas o recursos."),
                    new TipoSolicitud("Soporte", "Consulta o asistencia general."));
            }

            if (!await context.Usuarios.AnyAsync())
            {
                await context.Usuarios.AddRangeAsync(
                    new Usuario(
                        "Administrador Demo",
                        "admin@demo.local",
                        passwordHasher.Hash("Admin123!"),
                        Roles.Administrador),
                    new Usuario(
                        "Analista Demo",
                        "analista@demo.local",
                        passwordHasher.Hash("Analista123!"),
                        Roles.Analista),
                    new Usuario(
                        "Solicitante Demo",
                        "solicitante@demo.local",
                        passwordHasher.Hash("Solicitante123!"),
                        Roles.Solicitante));
            }

            await context.SaveChangesAsync();
        }
    }
}
