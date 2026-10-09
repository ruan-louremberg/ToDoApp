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
        var userId = Guid.NewGuid();
        var task = new ToDo("Deleted", null, Priority.Low, null);
        task.SoftDelete();
        var repository = new Mock<IToDoRepository>();
        repository
            .Setup(item => item.GetTrashAsync(userId, 
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ToDo> { task });
        repository
            .Setup(item => item.CountTrashAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await new GetTrashUseCase(repository.Object).ExecuteAsync(userId);

        Assert.Single(result.Items);
        Assert.Equal(task.Id, result.Items[0].Id);
        Assert.Equal("Deleted", result.Items[0].Title);
    }

    [Fact(DisplayName = "Retorna lista vazia quando a lixeira está vazia")]
    public async Task GivenEmptyTrash_WhenGettingTrash_ThenReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IToDoRepository>();
        repository
            .Setup(item => item.GetTrashAsync(userId, 
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ToDo>());
        repository
            .Setup(item => item.CountTrashAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await new GetTrashUseCase(repository.Object).ExecuteAsync(userId);

        Assert.Empty(result.Items);
    }
}