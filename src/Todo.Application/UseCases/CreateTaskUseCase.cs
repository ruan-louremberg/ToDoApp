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

    public async Task<Result<ToDo>> ExecuteAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Title.Length < 3 || string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Fail("O título da tarefa deve ter pelo menos 3 caracteres e não pode ser vazio.");
        }

        if (request.CategoryId.HasValue && !await _categoryRepository.ExistsAsync(request.CategoryId.Value, cancellationToken))
        {
            return Result.Fail("A categoria especificada não existe.");
        }


        
        var task = new ToDo(request.Title, request.Description, request.Priority, request.DueDate, request.CategoryId);
        await _toDoRepository.AddAsync(task, cancellationToken);
        return task;
    }
}