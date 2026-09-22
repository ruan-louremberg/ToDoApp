using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("O nome da categoria é obrigatório.")
            .MinimumLength(2)
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