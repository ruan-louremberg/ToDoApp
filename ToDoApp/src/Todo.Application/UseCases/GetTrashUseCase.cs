using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class GetTrashUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public GetTrashUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<List<TaskResponseList>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = await _toDoRepository.GetTrashAsync(cancellationToken);

        return tasks.Select(task => new TaskResponseList
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            Category = task.Category is not null
                ? new CategoryResponse(
                    task.Category.Id,
                    task.Category.Name,
                    task.Category.Color
                )
                : null
        }).ToList();
    }
}