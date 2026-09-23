using FluentResults;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class RestoreTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public RestoreTaskUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var restored = await _toDoRepository.RestoreAsync(
            id,
            cancellationToken);

        if (!restored)
        {
            return Result.Fail(new Error("Tarefa não encontrada na lixeira.")
                .WithMetadata("statusCode", 404));
        }

        return Result.Ok();
    }
}