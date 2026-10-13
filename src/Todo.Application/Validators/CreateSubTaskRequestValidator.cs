using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public sealed class CreateSubTaskRequestValidator : AbstractValidator<CreateSubTaskRequest>
{
    public CreateSubTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("O título da subtarefa é obrigatório.")
            .Must(title => title.Trim().Length <= 120)
            .WithMessage("O título da subtarefa deve ter no máximo 120 caracteres.");
    }
}
