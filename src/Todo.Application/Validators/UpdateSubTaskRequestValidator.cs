using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public sealed class UpdateSubTaskRequestValidator : AbstractValidator<UpdateSubTaskRequest>
{
    public UpdateSubTaskRequestValidator()
    {
        RuleFor(request => request)
            .Must(request => request.Title is not null || request.IsCompleted.HasValue)
            .WithMessage("Informe o título ou o estado de conclusão da subtarefa.");

        RuleFor(request => request.Title)
            .Cascade(CascadeMode.Stop)
            .Must(title => title is null || !string.IsNullOrWhiteSpace(title))
            .WithMessage("O título da subtarefa não pode ser vazio.")
            .Must(title => title is null || title.Trim().Length <= 120)
            .WithMessage("O título da subtarefa deve ter no máximo 120 caracteres.");
    }
}
