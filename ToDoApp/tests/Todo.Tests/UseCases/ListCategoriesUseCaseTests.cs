using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class ListCategoriesUseCaseTests
{
    [Fact(DisplayName = "Normaliza paginação e calcula total de categorias")]
    public async Task GivenOutOfRangePagination_WhenListingCategories_ThenUsesSafePagination()
    {
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetAllAsync(null, null, 1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Category>());
        repository.Setup(item => item.CountAsync(null, null, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var response = await new ListCategoriesUseCase(repository.Object).ExecuteAsync(new ListCategoriesRequest { Page = -1, PageSize = 0 });

        Assert.Equal(1, response.Page);
        Assert.Equal(1, response.PageSize);
        Assert.Equal(1, response.TotalPages);
    }

    [Fact(DisplayName = "Mapeia categorias no resultado")]
    public async Task GivenCategories_WhenListingCategories_ThenMapsResponseItems()
    {
        var category = new Category("Work", "#FFFFFF");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetAllAsync(null, null, 1, 20, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Category> { category });
        repository.Setup(item => item.CountAsync(null, null, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var response = await new ListCategoriesUseCase(repository.Object).ExecuteAsync(new ListCategoriesRequest());

        Assert.Single(response.Items);
        Assert.Equal(category.Id, response.Items[0].Id);
        Assert.Equal("#FFFFFF", response.Items[0].Color);
    }
}