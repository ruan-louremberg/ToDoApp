using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO;
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

        var result = await CreateSut(repository).ExecuteAsync(Guid.NewGuid(), new UpdateTaskRequest { Title = "Updated" });

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

        var result = await CreateSut(repository, categoryRepository).ExecuteAsync(task.Id, new UpdateTaskRequest
        {
            CategoryId = Guid.NewGuid()
        });

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
            Title = "Updated",
            Priority = Priority.High
        });

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", result.Value.Title);
        Assert.Equal(Priority.High, task.Priority);
        repository.Verify(item => item.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateTaskUseCase CreateSut(Mock<IToDoRepository> repository, Mock<ICategoryRepository>? categoryRepository = null)
    {
        var validator = new Mock<IValidator<UpdateTaskRequest>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<UpdateTaskRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        return new UpdateTaskUseCase(repository.Object, (categoryRepository ?? new Mock<ICategoryRepository>()).Object, validator.Object);
    }
}