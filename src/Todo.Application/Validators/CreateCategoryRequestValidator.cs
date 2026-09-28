using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(request => request.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("O nome da categoria é obrigatório.")
            .Must(name => name is null || name.Trim().Length >= 2)
            .WithMessage("O nome da categoria deve ter pelo menos 2 caracteres.")
            .MaximumLength(50)
            .WithMessage("O nome da categoria deve ter no máximo 50 caracteres.");

        RuleFor(request => request.Color)
            .NotEmpty()
            .WithMessage("A cor da categoria é obrigatória.")
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("A cor deve estar no formato hexadecimal #RRGGBB.");
    }
}
