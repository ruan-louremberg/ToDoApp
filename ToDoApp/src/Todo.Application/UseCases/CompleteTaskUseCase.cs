using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.UseCases;

    public class CompleteTaskUseCase
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IValidator<CompleteTaskRequest> _validator;

        public CompleteTaskUseCase(
            IToDoRepository toDoRepository,
            IValidator<CompleteTaskRequest> validator)
        {
            _toDoRepository = toDoRepository;
            _validator = validator;
        }

        public async Task<Result<CompleteTaskResponse>> ExecuteAsync(Guid id, CompleteTaskRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.Select(error =>
                    new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
            }

            var task = await _toDoRepository.GetByIdAsync(id, cancellationToken);   

            if (task == null)
            {
                return Result.Fail(new Error("Tarefa não encontrada.")
                    .WithMetadata("statusCode", 404));
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