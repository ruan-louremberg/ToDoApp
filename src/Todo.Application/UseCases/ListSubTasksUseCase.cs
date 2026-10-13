using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class ListSubTasksUseCase(IToDoRepository toDoRepository)
{
    public async Task<Result<IReadOnlyList<SubTaskResponse>>> ExecuteAsync(
        Guid userId,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await toDoRepository.GetByIdWithSubtasksAsync(userId, taskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail<IReadOnlyList<SubTaskResponse>>(
                new Error("Tarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        IReadOnlyList<SubTaskResponse> response = task.Subtasks
            .OrderBy(subtask => subtask.Position)
            .Select(SubTaskResponse.FromEntity)
            .ToList();
        return Result.Ok(response);
    }
}
