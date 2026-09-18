using Moq;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class GetTrashCategoryUseCaseTests
{
    [Fact(DisplayName = "Retorna categorias da lixeira mapeadas")]
    public async Task GivenDeletedCategories_WhenGettingTrash_ThenReturnsMappedCategories()
    {
        var category = new Category("Deleted", "#FFFFFF");
        category.SoftDelete();
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetTrashAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Category> { category });

        var result = await new GetTrashCategoryUseCase(repository.Object).ExecuteAsync();

        Assert.Single(result);
        Assert.Equal(category.Id, result[0].Id);
        Assert.Equal("Deleted", result[0].Name);
    }

    [Fact(DisplayName = "Retorna lista vazia quando a lixeira de categorias está vazia")]
    public async Task GivenEmptyTrash_WhenGettingTrash_ThenReturnsEmptyList()
    {
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetTrashAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Category>());

        var result = await new GetTrashCategoryUseCase(repository.Object).ExecuteAsync();

        Assert.Empty(result);
    }
}