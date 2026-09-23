using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class ListTasksUseCaseTests
{
    [Fact(DisplayName = "Normaliza paginação e calcula total de páginas")]
    public async Task GivenOutOfRangePagination_WhenListingTasks_ThenUsesSafePagination()
    {
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetAllAsync(null, null, null, null, "CreatedAt", "desc", 1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ToDo>());
        repository.Setup(item => item.CountAsync(null, null, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(201);

        var response = await new ListTasksUseCase(repository.Object).ExecuteAsync(new ListTasksRequest { Page = 0, PageSize = 200 });

        Assert.Equal(1, response.Page);
        Assert.Equal(100, response.PageSize);
        Assert.Equal(3, response.TotalPages);
    }

    [Fact(DisplayName = "Mapeia tarefas e categorias no resultado")]
    public async Task GivenTasks_WhenListingTasks_ThenMapsResponseItems()
    {
        var category = new Category("Work", "#FFFFFF");
        var task = new ToDo("Task", "Description", Priority.High, null);
        task.SetCategory(category);
        var repository = new Mock<IToDoRepository>();
        repository.Setup(item => item.GetAllAsync(It.IsAny<Status?>(), It.IsAny<Priority?>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ToDo> { task });
        repository.Setup(item => item.CountAsync(It.IsAny<Status?>(), It.IsAny<Priority?>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var response = await new ListTasksUseCase(repository.Object).ExecuteAsync(new ListTasksRequest());

        Assert.Single(response.Items);
        Assert.Equal(category.Id, response.Items[0].Category!.Id);
        Assert.Equal("Task", response.Items[0].Title);
    }
}