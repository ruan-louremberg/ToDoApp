using Moq;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class DeleteTaskUseCaseTests
{
    [Fact(DisplayName = "Retorna não encontrado quando a tarefa não existe")]
    public async Task GivenMissingTask_WhenDeletingTask_ThenReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ToDo?)null);

        var result = await new DeleteTaskUseCase(repository.Object).ExecuteAsync(userId, Guid.NewGuid());

        Assert.True(result.IsFailed);
        repository.Verify(item => item.DeleteAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Exclui tarefa existente")]
    public async Task GivenExistingTask_WhenDeletingTask_ThenDeletesTask()
    {
        var userId = Guid.NewGuid();
        var task = new ToDo("Task", null, Priority.Low, null);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await new DeleteTaskUseCase(repository.Object).ExecuteAsync(userId, task.Id);

        Assert.True(result.IsSuccess);
        repository.Verify(item => item.DeleteAsync(userId, task.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}