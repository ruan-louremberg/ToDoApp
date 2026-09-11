using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.UseCases;

    public class CompleteTaskUseCase
    {
        private readonly IToDoRepository _toDoRepository;

        public CompleteTaskUseCase(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<Result<CompleteTaskResponse>> ExecuteAsync(Guid id, CompleteTaskRequest request, CancellationToken cancellationToken = default)
        {
            var task = await _toDoRepository.GetByIdAsync(id, cancellationToken);   

            if (task == null)
            {
                return Result.Fail(new Error("Tarefa não encontrada."));
            }

            task.ChangeStatus(request.Status);

            await _toDoRepository.UpdateAsync(task, cancellationToken);

            var response = new CompleteTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CategoryId = task.CategoryId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CompletedAt = task.CompletedAt
            };
            return Result.Ok(response);
        }
    }