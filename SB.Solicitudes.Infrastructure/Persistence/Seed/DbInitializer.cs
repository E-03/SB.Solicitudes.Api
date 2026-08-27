using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;

namespace SB.Solicitudes.Infrastructure.Persistence.Seed
{
    public static class DbInitializer
    {
        private const string EntidadesGubernamentalesResourceName =
            "SB.Solicitudes.Infrastructure.Persistence.Seed.entidades-gubernamentales.json";

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
                        passwordHasher.Hash("Admin1234"),
                        Roles.Administrador),
                    new Usuario(
                        "Analista Demo",
                        "analista@demo.local",
                        passwordHasher.Hash("Analista1234"),
                        Roles.Analista),
                    new Usuario(
                        "Solicitante Demo",
                        "solicitante@demo.local",
                        passwordHasher.Hash("Solicitante1234"),
                        Roles.Solicitante));
            }

            await context.SaveChangesAsync();

            await SeedEntidadesGubernamentalesAsync(context);
        }

        private static async Task SeedEntidadesGubernamentalesAsync(AppDbContext context)
        {
            if (await context.EntidadesGubernamentales.AnyAsync())
            {
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();

            await using var stream = assembly.GetManifestResourceStream(
                EntidadesGubernamentalesResourceName);

            if (stream is null)
            {
                return;
            }

            var registros = await JsonSerializer.DeserializeAsync<List<EntidadGubernamentalSeedRecord>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (registros is null || registros.Count == 0)
            {
                return;
            }

            var entidades = registros
                .Select(r => new EntidadGubernamental(
                    r.Nombre,
                    r.Categoria,
                    r.PoderDelEstado,
                    r.Sector))
                .ToList();

            await context.EntidadesGubernamentales.AddRangeAsync(entidades);

            await context.SaveChangesAsync();
        }

        private sealed record EntidadGubernamentalSeedRecord(
            string Nombre,
            string Categoria,
            string PoderDelEstado,
            string Sector);
    }
}
