using FluentValidation;
using ToDoApp.Application.DTO.Auth;

namespace ToDoApp.Application.Validators;

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .Must(EmailAddressRules.HasValidDomain)
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(320)
            .WithMessage("O e-mail deve ter no máximo 320 caracteres.");
    }
}
