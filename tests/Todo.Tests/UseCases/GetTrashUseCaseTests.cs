using Moq;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class GetTrashUseCaseTests
{
    [Fact(DisplayName = "Retorna tarefas da lixeira mapeadas")]
    public async Task GivenDeletedTasks_WhenGettingTrash_ThenReturnsMappedTasks()
    {
        var task = new ToDo("Deleted", null, Priority.Low, null);
        task.SoftDelete();
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetTrashAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ToDo> { task });

        var result = await new GetTrashUseCase(repository.Object).ExecuteAsync();

        Assert.Single(result);
        Assert.Equal(task.Id, result[0].Id);
        Assert.Equal("Deleted", result[0].Title);
    }

    [Fact(DisplayName = "Retorna lista vazia quando a lixeira está vazia")]
    public async Task GivenEmptyTrash_WhenGettingTrash_ThenReturnsEmptyList()
    {
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetTrashAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ToDo>());

        var result = await new GetTrashUseCase(repository.Object).ExecuteAsync();

        Assert.Empty(result);
    }
}