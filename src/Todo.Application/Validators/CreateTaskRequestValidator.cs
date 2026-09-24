using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("O título da tarefa é obrigatório.")
            .Must(title => title is null || title.Trim().Length >= 3)
            .WithMessage("O título da tarefa deve ter pelo menos 3 caracteres.")
            .MaximumLength(120)
            .WithMessage("O título da tarefa deve ter no máximo 120 caracteres.");

        RuleFor(request => request.Priority)
            .IsInEnum()
            .WithMessage("A prioridade informada é inválida.");

        RuleFor(request => request.Description)
            .Must(description => description is null || description.Trim().Length >= 10)
            .WithMessage("A descrição da tarefa deve ter pelo menos 10 caracteres.")
            .MaximumLength(1000)
            .WithMessage("A descrição da tarefa deve ter no máximo 1000 caracteres.");

        RuleFor(request => request.DueDate)
            .Must(dueDate => !dueDate.HasValue || dueDate.Value >= DateTime.UtcNow)
            .WithMessage("A data de vencimento não pode ser no passado.");
    }
}
