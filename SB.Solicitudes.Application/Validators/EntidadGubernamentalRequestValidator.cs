using FluentValidation;
using SB.Solicitudes.Application.DTOs.EntidadesGubernamentales;

namespace SB.Solicitudes.Application.Validators
{
    public sealed class EntidadGubernamentalRequestValidator
        : AbstractValidator<EntidadGubernamentalRequest>
    {
        public EntidadGubernamentalRequestValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.Categoria)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.PoderDelEstado)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Sector)
                .NotEmpty()
                .MaximumLength(150);
        }
    }
}
