using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SB.Solicitudes.Application.Interfaces.Persistence;
using SB.Solicitudes.Domain.Interfaces.Persistence;
using SB.Solicitudes.Infrastructure.Persistence;
using SB.Solicitudes.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Infrastructure
{
    public static class InfrastructureServiceConfiguration
    {
        public static void AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            ConfigureDatabase(services, configuration);
            AddRepositories(services);
        }

        private static void ConfigureDatabase(IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        private static void AddRepositories(
            IServiceCollection services)
        {
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));
   
            services.AddScoped<
                IUsuarioRepository,
                UsuarioRepository>();
           
            services.AddScoped<
                IAreaRepository,
                AreaRepository>();

            services.AddScoped<
                IComentarioRepository,
                ComentarioRepository>();

            services.AddScoped<
                IHistorialEstadoRepository,
                HistorialEstadoRepository>();

            services.AddScoped<
                INotificacionRepository,
                NotificacionRepository>();

            services.AddScoped<
                ISolicitudRepository,
                SolicitudRepository>();

            services.AddScoped<
                ITipoSolicitudRepository,
                TipoSolicitudRepository>();
        }
    }
}
   
