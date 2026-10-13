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
        var userId = Guid.NewGuid();
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetDeletedByIdAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await new RestoreCategoryUseCase(repository.Object).ExecuteAsync(userId, Guid.NewGuid());

        Assert.True(result.IsFailed);
    }

    [Fact(DisplayName = "Retorna conflito quando o nome já está em uso")]
    public async Task GivenDuplicateCategoryName_WhenRestoringCategory_ThenReturnsConflict()
    {
        var userId = Guid.NewGuid();
        var deleted = new Category("Work", "#FFFFFF");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetDeletedByIdAsync(userId, deleted.Id, It.IsAny<CancellationToken>())).ReturnsAsync(deleted);
        repository.Setup(item => item.GetByNameAsync(userId, "Work", It.IsAny<CancellationToken>())).ReturnsAsync(new Category("Work", "#000000"));

        var result = await new RestoreCategoryUseCase(repository.Object).ExecuteAsync(userId, deleted.Id);

        Assert.True(result.IsFailed);
        Assert.Equal(409, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Restaura categoria sem conflito de nome")]
    public async Task GivenAvailableCategoryName_WhenRestoringCategory_ThenRestoresCategory()
    {
        var userId = Guid.NewGuid();
        var deleted = new Category("Work", "#FFFFFF");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetDeletedByIdAsync(userId, deleted.Id, It.IsAny<CancellationToken>())).ReturnsAsync(deleted);
        repository.Setup(item => item.GetByNameAsync(userId, "Work", It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);
        repository.Setup(item => item.RestoreAsync(userId, deleted.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await new RestoreCategoryUseCase(repository.Object).ExecuteAsync(userId, deleted.Id);

        Assert.True(result.IsSuccess);
        repository.Verify(item => item.RestoreAsync(userId, deleted.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}