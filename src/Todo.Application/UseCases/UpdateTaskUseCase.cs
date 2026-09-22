using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;


namespace ToDoApp.Application.UseCases;

public class UpdateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<UpdateTaskRequest> _validator;

    public UpdateTaskUseCase(
        IToDoRepository toDoRepository,
        ICategoryRepository categoryRepository,
        IValidator<UpdateTaskRequest> validator)
    {
        _toDoRepository = toDoRepository;
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Result<TaskResponse>> ExecuteAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var task = await _toDoRepository.GetByIdAsync(id, cancellationToken);

        if (task == null)
        {
            return Result.Fail(new Error("Tarefa não encontrada.")
                .WithMetadata("statusCode", 404));
        }

        var optionalDescription = request.Description;
        var optionalDueDate = request.DueDate;
        var optionalCategoryId = request.CategoryId;

        if (optionalCategoryId.IsSet && optionalCategoryId.Value is not null && optionalCategoryId.Value.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(optionalCategoryId.Value.Value, cancellationToken);
            if (category is null)
            {
                return Result.Fail(new Error("Categoria não encontrada.")
                    .WithMetadata("statusCode", 404));
            }
        }

        var title = request.Title.IsSet ? request.Title.Value : null;
        var priority = request.Priority.IsSet ? request.Priority.Value : null;

        task.SetUpdate(
            title,
            optionalDescription,
            priority,
            optionalDueDate,
            optionalCategoryId
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