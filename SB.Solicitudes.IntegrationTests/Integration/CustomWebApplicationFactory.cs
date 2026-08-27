using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Domain.Common;
using SB.Solicitudes.Domain.Entities;
using SB.Solicitudes.Infrastructure.Persistence;

namespace SB.Solicitudes.IntegrationTests.Integration
{
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string AdminEmail = "admin@test.local";
        public const string AnalistaEmail = "analista@test.local";
        public const string SolicitanteEmail = "solicitante@test.local";
        public const string Password = "Password123!";

        private readonly string _databaseName = $"SB_Solicitudes_Tests_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            context.Database.EnsureCreated();

            SeedTestData(context, passwordHasher);

            return host;
        }

        private static void SeedTestData(AppDbContext context, IPasswordHasher passwordHasher)
        {
            var area = new Area("Tecnología");
            var tipo = new TipoSolicitud("Incidente", "Falla de un servicio existente.");

            context.Areas.Add(area);
            context.TiposSolicitud.Add(tipo);

            context.Usuarios.AddRange(
                new Usuario("Administrador Test", AdminEmail, passwordHasher.Hash(Password), Roles.Administrador),
                new Usuario("Analista Test", AnalistaEmail, passwordHasher.Hash(Password), Roles.Analista),
                new Usuario("Solicitante Test", SolicitanteEmail, passwordHasher.Hash(Password), Roles.Solicitante));

            context.SaveChanges();
        }
    }
}
