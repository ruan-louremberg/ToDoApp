using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class ListTasksUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public ListTasksUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;    
    }

    public async Task<ListTasksResponse> ExecuteAsync(ListTasksRequest request, CancellationToken cancellationToken = default)
    {
        var safePage = Math.Max(request.Page, 1);
        var safePageSize = Math.Clamp(request.PageSize, 1, 100);

        var tasks = await _toDoRepository.GetAllAsync(
            request.Status,
            request.Priority,
            request.CategoryId,
            request.Search,
            request.SortBy,
            request.SortDirection,
            safePage,
            safePageSize,
            cancellationToken
        );

        var totalItems = await _toDoRepository.CountAsync(
            request.Status,
            request.Priority,
            request.CategoryId,
            request.Search,
            cancellationToken
        );
        var totalPages = (int)Math.Ceiling((double)totalItems / safePageSize);

        return new ListTasksResponse
        {
            Items = tasks.Select(task => new TaskResponseList
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
            }).ToList(),

            Page = safePage,
            PageSize = safePageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}