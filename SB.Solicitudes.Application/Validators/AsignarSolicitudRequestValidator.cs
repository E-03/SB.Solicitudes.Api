using FluentValidation;
using SB.Solicitudes.Application.DTOs.Solicitudes;

namespace SB.Solicitudes.Application.Validators
{
    public sealed class AsignarSolicitudRequestValidator : AbstractValidator<AsignarSolicitudRequest>
    {
        public AsignarSolicitudRequestValidator()
        {
            RuleFor(x => x.ResponsableId)
                .GreaterThan(0)
                .When(x => x.ResponsableId.HasValue);
        }
    }
}
