using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class CreateSubTaskUseCase(
    IToDoRepository toDoRepository,
    IValidator<CreateSubTaskRequest> validator)
{
    public async Task<Result<SubTaskResponse>> ExecuteAsync(
        Guid userId,
        Guid taskId,
        CreateSubTaskRequest request,
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

        var subtask = task.AddSubtask(request.Title.Trim());
        await toDoRepository.AddSubtaskAsync(subtask, cancellationToken);
        return Result.Ok(SubTaskResponse.FromEntity(subtask));
    }
}
