using FluentResults;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class DeleteSubTaskUseCase(IToDoRepository toDoRepository)
{
    public async Task<Result> ExecuteAsync(
        Guid userId,
        Guid taskId,
        Guid subtaskId,
        CancellationToken cancellationToken = default)
    {
        var task = await toDoRepository.GetByIdWithSubtasksAsync(userId, taskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(
                new Error("Tarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        if (!task.RemoveSubtask(subtaskId))
        {
            return Result.Fail(
                new Error("Subtarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        await toDoRepository.UpdateAsync(task, cancellationToken);
        return Result.Ok();
    }
}
