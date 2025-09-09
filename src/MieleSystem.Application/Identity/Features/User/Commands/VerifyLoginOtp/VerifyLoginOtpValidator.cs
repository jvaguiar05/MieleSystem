using FluentValidation;

namespace MieleSystem.Application.Identity.Features.User.Commands.VerifyLoginOtp;

public sealed class VerifyLoginOtpValidator : AbstractValidator<VerifyLoginOtpCommand>
{
    public VerifyLoginOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .WithMessage("O código OTP é obrigatório.")
            .Length(6)
            .WithMessage("O código OTP deve ter 6 dígitos.")
            .Matches(@"^\d{6}$")
            .WithMessage("O código OTP deve conter apenas números.");
    }
}
