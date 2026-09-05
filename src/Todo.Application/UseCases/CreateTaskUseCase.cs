using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class CreateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateTaskUseCase(IToDoRepository toDoRepository, ICategoryRepository categoryRepository)
    {
        _toDoRepository = toDoRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<TaskResponse>> ExecuteAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Title.Length < 3 || string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Fail("O título da tarefa deve ter pelo menos 3 caracteres e não pode ser vazio.");
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
        
        var task = new ToDo(request.Title, request.Description, request.Priority, request.DueDate, request.CategoryId);
        if (category != null)
        {
            task.SetCategory(category);
        }
        await _toDoRepository.AddAsync(task, cancellationToken);
        var taskResponse = new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            (int)task.Priority,
            task.DueDate,
            category != null ? new CategoryResponse(category.Id, category.Name, category.Color) : null
        );
        return Result.Ok(taskResponse);
    }
}