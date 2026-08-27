using FluentValidation;
using SB.Solicitudes.Application.DTOs.Auth;

namespace SB.Solicitudes.Application.Validators
{
    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Correo)
                .NotEmpty()
                .MaximumLength(100)
                .EmailAddress();

            RuleFor(x => x.Contraseña)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
