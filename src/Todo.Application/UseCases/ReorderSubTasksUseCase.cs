using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class ReorderSubTasksUseCase(
    IToDoRepository toDoRepository,
    IValidator<ReorderSubTasksRequest> validator)
{
    public async Task<Result> ExecuteAsync(
        Guid userId,
        Guid taskId,
        ReorderSubTasksRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Fail(validation.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var task = await toDoRepository.GetByIdWithSubtasksAsync(userId, taskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(
                new Error("Tarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        if (!task.ReorderSubtasks(request.SubtaskIds!))
        {
            return Result.Fail(
                new Error("A ordem informada deve conter todas as subtarefas da tarefa, sem identificadores extras.")
                    .WithMetadata("statusCode", 400));
        }

        await toDoRepository.UpdateAsync(task, cancellationToken);
        return Result.Ok();
    }
}
