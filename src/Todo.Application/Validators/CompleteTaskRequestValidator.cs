using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class CompleteTaskRequestValidator : AbstractValidator<CompleteTaskRequest>
{
    public CompleteTaskRequestValidator()
    {
        RuleFor(request => request.Status)
            .IsInEnum()
            .WithMessage("O status informado é inválido.");
    }
}
