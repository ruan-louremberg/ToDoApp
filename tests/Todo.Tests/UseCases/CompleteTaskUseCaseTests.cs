using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Exceptions;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class CompleteTaskUseCaseTests
{
    [Fact(DisplayName = "Retorna não encontrado quando a tarefa não existe")]
    public async Task GivenMissingTask_WhenChangingStatus_ThenReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ToDo?)null);

        var result = await CreateSut(repository).ExecuteAsync(userId, Guid.NewGuid(), new CompleteTaskRequest { Status = Status.Completed });

        Assert.True(result.IsFailed);
        Assert.Equal(404, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Atualiza status e retorna a tarefa concluída")]
    public async Task GivenExistingTask_WhenChangingStatusToCompleted_ThenUpdatesAndReturnsTask()
    {
        var userId = Guid.NewGuid();
        var task = new ToDo("Task", null, Priority.Medium, null);
        task.ChangeStatus(Status.InProgress);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await CreateSut(repository).ExecuteAsync(userId, task.Id, new CompleteTaskRequest { Status = Status.Completed });

        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Completed, result.Value.Status);
        Assert.NotNull(result.Value.CompletedAt);
        repository.Verify(item => item.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Retorna conflito para transição de status não permitida")]
    public async Task GivenDisallowedTransition_WhenChangingStatus_ThenReturnsConflict()
    {
        var userId = Guid.NewGuid();
        var task = new ToDo("Task", null, Priority.Medium, null);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await CreateSut(repository).ExecuteAsync(userId, 
            task.Id,
            new CompleteTaskRequest { Status = Status.Completed });

        Assert.True(result.IsFailed);
        Assert.Equal(409, result.Errors[0].Metadata["statusCode"]);
        repository.Verify(item => item.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CompleteTaskUseCase CreateSut(Mock<IToDoRepository> repository)
    {
        var validator = new Mock<IValidator<CompleteTaskRequest>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<CompleteTaskRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        return new CompleteTaskUseCase(repository.Object, validator.Object);
    }
}