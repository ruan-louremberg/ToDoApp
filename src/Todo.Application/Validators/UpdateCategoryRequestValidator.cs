using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(request => request.Name.Value)
            .NotEmpty()
            .When(request => request.Name.IsSet && request.Name.Value is not null)
            .WithMessage("O nome da categoria é obrigatório.")
            .MinimumLength(2)
            .When(request => request.Name.IsSet && request.Name.Value is not null)
            .WithMessage("O nome da categoria deve ter pelo menos 2 caracteres.")
            .MaximumLength(50)
            .When(request => request.Name.IsSet && request.Name.Value is not null)
            .WithMessage("O nome da categoria deve ter no máximo 50 caracteres.");

        RuleFor(request => request.Color.Value)
            .NotEmpty()
            .When(request => request.Color.IsSet && request.Color.Value is not null)
            .WithMessage("A cor da categoria é obrigatória.")
            .Matches("^#[0-9A-Fa-f]{6}$")
            .When(request => request.Color.IsSet && request.Color.Value is not null)
            .WithMessage("A cor deve estar no formato hexadecimal #RRGGBB.");
    }
}