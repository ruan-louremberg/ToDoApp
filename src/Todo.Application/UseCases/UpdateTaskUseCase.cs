using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;


namespace ToDoApp.Application.UseCases;

public class UpdateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateTaskUseCase(IToDoRepository toDoRepository, ICategoryRepository categoryRepository)
    {
        _toDoRepository = toDoRepository;
        _categoryRepository = categoryRepository;
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

         Category? category = null;
        if (request.CategoryId.HasValue)
        {
            category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                return Result.Fail("Categoria não encontrada.");
            }
        }

        if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
        {
            return Result.Fail("A data de vencimento não pode ser no passado.");
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