using Moq;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class DeleteCategoryUseCaseTests
{
    [Fact(DisplayName = "Retorna não encontrado quando a categoria não existe")]
    public async Task GivenMissingCategory_WhenDeletingCategory_ThenReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await new DeleteCategoryUseCase(repository.Object).ExecuteAsync(userId, Guid.NewGuid());

        Assert.True(result.IsFailed);
        repository.Verify(item => item.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Exclui categoria logicamente")]
    public async Task GivenExistingCategory_WhenDeletingCategory_ThenMarksAndPersistsDeletion()
    {
        var userId = Guid.NewGuid();
        var category = new Category("Work", "#FFFFFF");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetByIdAsync(userId, category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        var result = await new DeleteCategoryUseCase(repository.Object).ExecuteAsync(userId, category.Id);

        Assert.True(result.IsSuccess);
        Assert.True(category.IsDeleted);
        repository.Verify(item => item.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }
}