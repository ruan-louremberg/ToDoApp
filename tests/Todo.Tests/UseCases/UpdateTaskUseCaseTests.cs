using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Domain;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class UpdateTaskUseCaseTests
{
    [Fact(DisplayName = "Retorna não encontrado quando a tarefa não existe")]
    public async Task GivenMissingTask_WhenUpdatingTask_ThenReturnsNotFound()
    {
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ToDo?)null);

        var result = await CreateSut(repository).ExecuteAsync(Guid.NewGuid(), new UpdateTaskRequest { Title = Optional<string>.Of("Updated") });

        Assert.True(result.IsFailed);
        Assert.Equal(404, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Retorna não encontrado quando a nova categoria não existe")]
    public async Task GivenMissingCategory_WhenUpdatingTask_ThenReturnsNotFound()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        var categoryRepository = new Mock<ICategoryRepository>();
        categoryRepository.Setup(item => item.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var request = new UpdateTaskRequest
        {
            CategoryId = Optional<Guid?>.Of(Guid.NewGuid())
        };

        var result = await CreateSut(repository, categoryRepository).ExecuteAsync(task.Id, request);

        Assert.True(result.IsFailed);
        Assert.Equal(404, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Atualiza e persiste tarefa existente")]
    public async Task GivenExistingTask_WhenUpdatingTask_ThenPersistsUpdatedValues()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await CreateSut(repository).ExecuteAsync(task.Id, new UpdateTaskRequest
        {
            Title = Optional<string>.Of("Updated"),
            Priority = Optional<Priority?>.Of(Priority.High)
        });

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", result.Value.Title);
        Assert.Equal(Priority.High, task.Priority);
        repository.Verify(item => item.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Mantém a due date quando o campo não veio no PATCH parcial")]
    public async Task GivenExistingDueDate_WhenPatchDoesNotIncludeDueDate_ThenKeepsCurrentValue()
    {
        var existingDueDate = DateTime.UtcNow.AddDays(3);
        var task = new ToDo("Task", null, Priority.Low, existingDueDate);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await CreateSut(repository).ExecuteAsync(task.Id, new UpdateTaskRequest
        {
            Title = Optional<string>.Of("Updated title")
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(existingDueDate, task.DueDate);
    }

    [Fact(DisplayName = "Permite limpar a due date quando o campo vier explicitamente como null")]
    public async Task GivenExistingDueDate_WhenPatchIncludesDueDateNull_ThenClearsTheValue()
    {
        var task = new ToDo("Task", null, Priority.Low, DateTime.UtcNow.AddDays(3));
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await CreateSut(repository).ExecuteAsync(task.Id, new UpdateTaskRequest
        {
            DueDate = Optional<DateTime?>.Of(null)
        });

        Assert.True(result.IsSuccess);
        Assert.Null(task.DueDate);
    }

    private static UpdateTaskUseCase CreateSut(Mock<IToDoRepository> repository, Mock<ICategoryRepository>? categoryRepository = null)
    {
        var validator = new Mock<IValidator<UpdateTaskRequest>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<UpdateTaskRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        return new UpdateTaskUseCase(repository.Object, (categoryRepository ?? new Mock<ICategoryRepository>()).Object, validator.Object);
    }
}