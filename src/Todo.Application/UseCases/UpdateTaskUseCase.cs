using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.UseCases;

public class UpdateTaskUseCase
{
    private readonly IToDoRepository _toDoRepository;

    public UpdateTaskUseCase(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result<UpdateTaskRequest>> ExecuteAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {

        var task = await _toDoRepository.GetByIdAsync(id, cancellationToken);


        if (task == null)
        {
            return Result.Fail("Tarefa não encontrada.");
        }

        if (string.IsNullOrWhiteSpace(task.Title) || task.Title.Length < 3)
        {
            return Result.Fail("O título da tarefa deve ter pelo menos 3 caracteres e não pode ser vazio.");
        }

        task.SetUpdate(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CategoryId
        );

        await _toDoRepository.UpdateAsync(task, cancellationToken);

        return Result.Ok(TaskResponse)
        //fazer um response para enviar de volta o resultado correto
    }
}