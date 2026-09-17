using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class CreateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateTaskRequest> _validator;

    public CreateTaskUseCase(
        IToDoRepository toDoRepository,
        ICategoryRepository categoryRepository,
        IValidator<CreateTaskRequest> validator)
    {
        _toDoRepository = toDoRepository;
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Result<TaskResponse>> ExecuteAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        Category? category = null;
        if (request.CategoryId.HasValue)
        {
            category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                return Result.Fail(new Error("Categoria não encontrada.")
                    .WithMetadata("statusCode", 404));
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