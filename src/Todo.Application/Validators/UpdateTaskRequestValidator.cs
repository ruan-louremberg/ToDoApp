using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(request => request.Title.Value)
            .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
            .When(request => request.Title.IsSet && request.Title.Value is not null)
            .WithMessage("O título não pode ser vazio.")
            .Must(value => value is null || value.Trim().Length >= 3)
            .When(request => request.Title.IsSet && request.Title.Value is not null)
            .WithMessage("O título deve ter pelo menos 3 caracteres.")
            .MaximumLength(120)
            .When(request => request.Title.IsSet && request.Title.Value is not null)
            .WithMessage("O título deve ter no máximo 120 caracteres.");

        RuleFor(request => request.Description.Value)
            .Must(value => value is null || value.Trim().Length >= 10)
            .When(request => request.Description.IsSet && request.Description.Value is not null)
            .WithMessage("A descrição da tarefa deve ter pelo menos 10 caracteres.")
            .MaximumLength(1000)
            .When(request => request.Description.IsSet && request.Description.Value is not null)
            .WithMessage("A descrição da tarefa deve ter no máximo 1000 caracteres.");

        RuleFor(request => request.Priority.Value)
            .IsInEnum()
            .When(request => request.Priority.IsSet && request.Priority.Value is not null)
            .WithMessage("A prioridade informada é inválida.");

        RuleFor(request => request.DueDate.Value)
            .Must(dueDate => !dueDate.HasValue || dueDate.Value >= DateTime.UtcNow)
            .When(request => request.DueDate.IsSet && request.DueDate.Value is not null && request.DueDate.Value.HasValue)
            .WithMessage("A data de vencimento não pode ser no passado.");
    }
}
