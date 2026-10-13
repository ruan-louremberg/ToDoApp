using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class GetSubTaskUseCase(IToDoRepository toDoRepository)
{
    public async Task<Result<SubTaskResponse>> ExecuteAsync(
        Guid userId,
        Guid taskId,
        Guid subtaskId,
        CancellationToken cancellationToken = default)
    {
        var task = await toDoRepository.GetByIdWithSubtasksAsync(userId, taskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail<SubTaskResponse>(
                new Error("Tarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        var subtask = task.Subtasks.SingleOrDefault(item => item.Id == subtaskId);
        if (subtask is null)
        {
            return Result.Fail<SubTaskResponse>(
                new Error("Subtarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        return Result.Ok(SubTaskResponse.FromEntity(subtask));
    }
}
