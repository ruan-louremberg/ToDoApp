using Moq;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class RestoreCategoryUseCaseTests
{
    [Fact(DisplayName = "Retorna não encontrado quando a categoria não está na lixeira")]
    public async Task GivenCategoryNotInTrash_WhenRestoringCategory_ThenReturnsNotFound()
    {
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetDeletedByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await new RestoreCategoryUseCase(repository.Object).ExecuteAsync(Guid.NewGuid());

        Assert.True(result.IsFailed);
    }

    [Fact(DisplayName = "Retorna conflito quando o nome já está em uso")]
    public async Task GivenDuplicateCategoryName_WhenRestoringCategory_ThenReturnsConflict()
    {
        var deleted = new Category("Work", "#FFFFFF");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetDeletedByIdAsync(deleted.Id, It.IsAny<CancellationToken>())).ReturnsAsync(deleted);
        repository.Setup(item => item.GetByNameAsync("Work", It.IsAny<CancellationToken>())).ReturnsAsync(new Category("Work", "#000000"));

        var result = await new RestoreCategoryUseCase(repository.Object).ExecuteAsync(deleted.Id);

        Assert.True(result.IsFailed);
        Assert.Equal(409, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Restaura categoria sem conflito de nome")]
    public async Task GivenAvailableCategoryName_WhenRestoringCategory_ThenRestoresCategory()
    {
        var deleted = new Category("Work", "#FFFFFF");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetDeletedByIdAsync(deleted.Id, It.IsAny<CancellationToken>())).ReturnsAsync(deleted);
        repository.Setup(item => item.GetByNameAsync("Work", It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);
        repository.Setup(item => item.RestoreAsync(deleted.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await new RestoreCategoryUseCase(repository.Object).ExecuteAsync(deleted.Id);

        Assert.True(result.IsSuccess);
        repository.Verify(item => item.RestoreAsync(deleted.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}