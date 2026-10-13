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
        Guid userId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await _toDoRepository.GetByIdAsync(
            userId,
            id,
            cancellationToken
        );

        if (task is null)
        {
            return Result.Fail(new Error("Tarefa não encontrada.")
                .WithMetadata("statusCode", 404));
        }

        await _toDoRepository.DeleteAsync(
            userId,
            id,
            cancellationToken
        );

        return Result.Ok();
    }
}
