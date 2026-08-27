using FluentValidation;
using SB.Solicitudes.Application.DTOs.Comentarios;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Application.Validators
{
    public sealed class CrearComentarioRequestValidator : AbstractValidator<CrearComentarioRequest>
    {
        public CrearComentarioRequestValidator()
        {
            RuleFor(x => x.Texto)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Visibilidad)
                .NotEmpty()
                .Must(x => Enum.TryParse<VisibilidadComentario>(x, true, out _))
                .WithMessage("La visibilidad debe ser Interno o Publico.");
        }
    }
}
