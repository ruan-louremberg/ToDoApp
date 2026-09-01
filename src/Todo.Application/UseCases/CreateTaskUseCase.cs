using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class CreateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public CreateTaskUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<ToDo> ExecuteAsync(string title, string? description, Priority priority, DateTime? dueDate)
    {
        var task = new ToDo(title, description, priority, dueDate);
        await _toDoRepository.AddAsync(task);
        return task;
    }
}