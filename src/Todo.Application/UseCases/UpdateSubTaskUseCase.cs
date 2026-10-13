using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class UpdateSubTaskUseCase(
    IToDoRepository toDoRepository,
    IValidator<UpdateSubTaskRequest> validator)
{
    public async Task<Result<SubTaskResponse>> ExecuteAsync(
        Guid userId,
        Guid taskId,
        Guid subtaskId,
        UpdateSubTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Fail<SubTaskResponse>(validation.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var task = await toDoRepository.GetByIdWithSubtasksAsync(userId, taskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail<SubTaskResponse>(
                new Error("Tarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        var title = request.Title?.Trim();
        if (!task.UpdateSubtask(subtaskId, title, request.IsCompleted))
        {
            return Result.Fail<SubTaskResponse>(
                new Error("Subtarefa não encontrada.").WithMetadata("statusCode", 404));
        }

        await toDoRepository.UpdateAsync(task, cancellationToken);
        var subtask = task.Subtasks.Single(item => item.Id == subtaskId);
        return Result.Ok(SubTaskResponse.FromEntity(subtask));
    }
}
