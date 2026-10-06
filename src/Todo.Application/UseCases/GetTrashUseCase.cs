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

    public async Task<ListTasksResponse> ExecuteAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        var tasks = await _toDoRepository.GetTrashAsync(
            safePage,
            safePageSize,
            cancellationToken);
        var totalItems = await _toDoRepository.CountTrashAsync(cancellationToken);

        return new ListTasksResponse
        {
            Items = tasks.Select(task => new TaskResponseList
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DeletedAt = task.DeletedAt,
                DueDate = task.DueDate,
                Category = task.Category is not null
                    ? new CategoryResponse(
                        task.Category.Id,
                        task.Category.Name,
                        task.Category.Color
                    )
                    : null
            }).ToList(),
            Page = safePage,
            PageSize = safePageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / safePageSize)
        };
    }
}