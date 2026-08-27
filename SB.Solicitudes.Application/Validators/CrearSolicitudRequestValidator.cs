using FluentValidation;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Application.Validators
{
    public sealed class CrearSolicitudRequestValidator : AbstractValidator<CrearSolicitudRequest>
    {
        public CrearSolicitudRequestValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .MaximumLength(4000);

            RuleFor(x => x.Prioridad)
                .NotEmpty()
                .Must(x => Enum.TryParse<PrioridadesSolicitud>(x, true, out _))
                .WithMessage("La prioridad debe ser Baja, Media, Alta o Critica.");

            RuleFor(x => x.AreaId)
                .GreaterThan(0);

            RuleFor(x => x.TipoSolicitudId)
                .GreaterThan(0);

            RuleFor(x => x.FechaCompromiso)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("La fecha de compromiso debe ser futura.");

            RuleFor(x => x.UrlEvidencia)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.UrlEvidencia));

            RuleFor(x => x.ReferenciaEvidencia)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.ReferenciaEvidencia));
        }
    }
}
