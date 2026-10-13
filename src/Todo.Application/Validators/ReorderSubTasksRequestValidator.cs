using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public sealed class ReorderSubTasksRequestValidator : AbstractValidator<ReorderSubTasksRequest>
{
    public ReorderSubTasksRequestValidator()
    {
        RuleFor(request => request.SubtaskIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Informe a ordem das subtarefas.")
            .NotEmpty()
            .WithMessage("A ordem das subtarefas não pode ser vazia.")
            .Must(ids => ids is not null && ids.Distinct().Count() == ids.Count)
            .WithMessage("A lista de subtarefas não pode conter identificadores duplicados.");
    }
}
