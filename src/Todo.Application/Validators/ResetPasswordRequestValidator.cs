using FluentValidation;
using ToDoApp.Application.DTO.Auth;

namespace ToDoApp.Application.Validators;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(request => request.Token)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("O token de redefinição é obrigatório.")
            .Length(64)
            .Matches("^[A-Fa-f0-9]{64}$")
            .WithMessage("O token de redefinição é inválido.");

        RuleFor(request => request.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .MinimumLength(8)
            .WithMessage("A senha deve ter pelo menos 8 caracteres.")
            .MaximumLength(100)
            .WithMessage("A senha deve ter no máximo 100 caracteres.");
    }
}
