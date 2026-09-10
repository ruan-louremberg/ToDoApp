using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;


namespace ToDoApp.Application.UseCases;

public class UpdateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public UpdateTaskUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result<TaskResponse>> ExecuteAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {

        var task = await _toDoRepository.GetByIdAsync(id, cancellationToken);


        if (task == null)
        {
            return Result.Fail("Tarefa não encontrada.");
        }

        if (request.Title != null)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length < 3)
            {
                return Result.Fail("O título da tarefa deve ter pelo menos 3 caracteres e não pode ser vazio.");
            }
        }

        task.SetUpdate(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CategoryId
        );

        await _toDoRepository.UpdateAsync(task, cancellationToken);

        var taskResponse = new TaskResponse(
            task.Id,
            task.Title.Trim(),
            task.Description,
            (int)task.Priority,
            task.DueDate,
            task.Category != null ? new CategoryResponse(task.Category.Id, task.Category.Name, task.Category.Color) : null
        );

        return Result.Ok(taskResponse);

    }
}