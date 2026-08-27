using FluentValidation;
using SB.Solicitudes.Application.DTOs.Solicitudes;
using SB.Solicitudes.Domain.Common;

namespace SB.Solicitudes.Application.Validators
{
    public sealed class CambiarEstadoRequestValidator : AbstractValidator<CambiarEstadoRequest>
    {
        public CambiarEstadoRequestValidator()
        {
            RuleFor(x => x.Estado)
                .NotEmpty()
                .Must(x => Enum.TryParse<EstadosSolicitud>(x, true, out _))
                .WithMessage("Estado inválido.");

            RuleFor(x => x.Comentario)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
