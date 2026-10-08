using FluentValidation;
using ToDoApp.Application.DTO.Auth;

namespace ToDoApp.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.");
    }
}
