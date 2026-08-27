using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Solicitudes.Application.Features.Auth;
using SB.Solicitudes.Application.Features.Catalogos;
using SB.Solicitudes.Application.Features.Dashboard;
using SB.Solicitudes.Application.Features.EntidadesGubernamentales;
using SB.Solicitudes.Application.Features.Notificacion;
using SB.Solicitudes.Application.Features.Solicitudes;
using SB.Solicitudes.Application.Interfaces.Services;

namespace SB.Solicitudes.Application
{
    public static class ApplicationServiceConfiguration
    {
        public static void AddApplication(
            this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(
                typeof(ApplicationServiceConfiguration).Assembly);

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISolicitudService, SolicitudService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ICatalogoService, CatalogoService>();
            services.AddScoped<IEntidadGubernamentalService, EntidadGubernamentalService>();
        }
    }
}
