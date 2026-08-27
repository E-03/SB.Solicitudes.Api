using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Application.Interfaces.Services;
using SB.Solicitudes.Infrastructure.CurrentUser;
using SB.Solicitudes.Infrastructure.Persistence;
using SB.Solicitudes.Infrastructure.Persistence.Repositories;
using SB.Solicitudes.Infrastructure.Security;

namespace SB.Solicitudes.Infrastructure
{
    public static class InfrastructureServiceConfiguration
    {
        public static void AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            ConfigureDatabase(services, configuration);
            ConfigureSecurity(services, configuration);
            AddRepositories(services);
        }

        private static void ConfigureDatabase(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "La cadena de conexión 'DefaultConnection' no está configurada.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(
                        typeof(AppDbContext).Assembly.FullName)));
        }

        private static void ConfigureSecurity(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.SectionName));

            services.AddHttpContextAccessor();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }

        private static void AddRepositories(
            IServiceCollection services)
        {
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IComentarioRepository, ComentarioRepository>();
            services.AddScoped<IHistorialEstadoRepository, HistorialEstadoRepository>();
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
            services.AddScoped<ISolicitudRepository, SolicitudRepository>();
            services.AddScoped<ITipoSolicitudRepository, TipoSolicitudRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
