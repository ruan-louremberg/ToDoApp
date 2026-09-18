using Moq;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class RestoreTaskUseCaseTests
{
    [Fact(DisplayName = "Retorna não encontrado quando a tarefa não está na lixeira")]
    public async Task GivenTaskNotInTrash_WhenRestoringTask_ThenReturnsNotFound()
    {
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.RestoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await new RestoreTaskUseCase(repository.Object).ExecuteAsync(Guid.NewGuid());

        Assert.True(result.IsFailed);
    }

    [Fact(DisplayName = "Restaura tarefa que está na lixeira")]
    public async Task GivenTaskInTrash_WhenRestoringTask_ThenReturnsSuccess()
    {
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.RestoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await new RestoreTaskUseCase(repository.Object).ExecuteAsync(Guid.NewGuid());

        Assert.True(result.IsSuccess);
    }
}