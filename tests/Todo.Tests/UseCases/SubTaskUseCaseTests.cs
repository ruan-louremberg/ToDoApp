using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class SubTaskUseCaseTests
{
    [Fact(DisplayName = "Cria subtarefa e persiste a tarefa agregada")]
    public async Task GivenValidRequest_WhenCreatingSubtask_ThenPersistsTaskAndReturnsSubtask()
    {
        var task = CreateTask();
        var repository = CreateRepository(task);
        var useCase = new CreateSubTaskUseCase(repository.Object, CreateValidator<CreateSubTaskRequest>());

        var result = await useCase.ExecuteAsync(
            Guid.NewGuid(), task.Id, new CreateSubTaskRequest { Title = "  First item " });

        Assert.True(result.IsSuccess);
        Assert.Equal("First item", result.Value.Title);
        Assert.Equal(0, result.Value.Position);
        repository.Verify(item => item.AddSubtaskAsync(
            It.Is<SubTask>(subtask => subtask.Title == "First item"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Retorna não encontrado quando a tarefa não existe")]
    public async Task GivenMissingTask_WhenListingSubtasks_ThenReturnsNotFound()
    {
        var repository = CreateRepository(null);
        var useCase = new ListSubTasksUseCase(repository.Object);

        var result = await useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.True(result.IsFailed);
        Assert.Equal(404, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Não persiste quando a subtarefa não pertence à tarefa")]
    public async Task GivenSubtaskFromAnotherTask_WhenUpdating_ThenReturnsNotFound()
    {
        var task = CreateTask();
        var repository = CreateRepository(task);
        var validator = CreateValidator<UpdateSubTaskRequest>();
        var useCase = new UpdateSubTaskUseCase(repository.Object, validator);

        var result = await useCase.ExecuteAsync(
            Guid.NewGuid(),
            task.Id,
            Guid.NewGuid(),
            new UpdateSubTaskRequest { IsCompleted = true });

        Assert.True(result.IsFailed);
        Assert.Equal(404, result.Errors[0].Metadata["statusCode"]);
        repository.Verify(item => item.UpdateAsync(It.IsAny<ToDo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Reordena subtarefas e persiste a tarefa")]
    public async Task GivenValidOrder_WhenReordering_ThenUpdatesPositionsAndPersists()
    {
        var task = CreateTask();
        var first = task.AddSubtask("First");
        var second = task.AddSubtask("Second");
        var repository = CreateRepository(task);
        var useCase = new ReorderSubTasksUseCase(
            repository.Object,
            CreateValidator<ReorderSubTasksRequest>());

        var result = await useCase.ExecuteAsync(
            Guid.NewGuid(),
            task.Id,
            new ReorderSubTasksRequest { SubtaskIds = [second.Id, first.Id] });

        Assert.True(result.IsSuccess);
        Assert.Equal(0, second.Position);
        Assert.Equal(1, first.Position);
        repository.Verify(item => item.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ToDo CreateTask() => new("Task", null, Priority.Low, null);

    private static Mock<IToDoRepository> CreateRepository(ToDo? task)
    {
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdWithSubtasksAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        return repository;
    }

    private static IValidator<T> CreateValidator<T>() where T : class
    {
        var validator = new Mock<IValidator<T>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        return validator.Object;
    }
}
