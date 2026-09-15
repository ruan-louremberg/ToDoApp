using FluentResults;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class DeleteTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public DeleteTaskUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await _toDoRepository.GetByIdAsync(
            id,
            cancellationToken
        );

        if (task is null)
        {
            return Result.Fail("Tarefa não encontrada.");
        }

        await _toDoRepository.DeleteAsync(
            id,
            cancellationToken
        );

        return Result.Ok();
    }
}
