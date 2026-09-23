using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .When(request => request.Title is not null)
            .WithMessage("O título não pode ser vazio.")
            .MinimumLength(3)
            .When(request => request.Title is not null)
            .WithMessage("O título deve ter pelo menos 3 caracteres.")
            .MaximumLength(120)
            .When(request => request.Title is not null)
            .WithMessage("O título deve ter no máximo 120 caracteres.");

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .When(request => request.Description is not null);

        RuleFor(request => request.Priority)
            .IsInEnum()
            .When(request => request.Priority.HasValue)
            .WithMessage("A prioridade informada é inválida.");

        RuleFor(request => request.DueDate)
            .Must(dueDate => !dueDate.HasValue || dueDate.Value >= DateTime.UtcNow)
            .WithMessage("A data de vencimento não pode ser no passado.");
    }
}
