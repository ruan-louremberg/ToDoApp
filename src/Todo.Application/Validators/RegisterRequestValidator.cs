using FluentValidation;
using ToDoApp.Application.DTO.Auth;

namespace ToDoApp.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("O nome é obrigatório.")
            .Must(name => name is not null && name.Trim().Length >= 2)
            .WithMessage("O nome deve ter pelo menos 2 caracteres.")
            .MaximumLength(120)
            .WithMessage("O nome deve ter no máximo 120 caracteres.");

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(320)
            .WithMessage("O e-mail deve ter no máximo 320 caracteres.");

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
