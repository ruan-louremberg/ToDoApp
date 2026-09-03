using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class ListTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public ListTaskUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result<List<ToDo>>> ExecuteAsync(ListTasksRequest request, CancellationToken cancellationToken = default)
    {
        return await _toDoRepository.GetAllAsync(cancellationToken);
    }
}