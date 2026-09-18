using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class CreateCategoryUseCaseTests
{
    [Fact(DisplayName = "Retorna erro quando os dados da categoria são inválidos")]
    public async Task GivenInvalidRequest_WhenCreatingCategory_ThenReturnsBadRequest()
    {
        var repository = new Mock<ICategoryRepository>();
        var validator = CreateValidator<CreateCategoryRequest>(false);

        var result = await new CreateCategoryUseCase(repository.Object, validator.Object).ExecuteAsync(new CreateCategoryRequest());

        Assert.True(result.IsFailed);
        repository.Verify(item => item.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Retorna conflito quando o nome já existe")]
    public async Task GivenExistingCategory_WhenCreatingCategory_ThenReturnsConflict()
    {
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(item => item.GetByNameAsync("Work", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category("Work", "#FFFFFF"));

        var result = await CreateSut(repository).ExecuteAsync(new CreateCategoryRequest { Name = "Work", Color = "#FFFFFF" });

        Assert.True(result.IsFailed);
        Assert.Equal(409, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Persiste uma categoria válida")]
    public async Task GivenValidRequest_WhenCreatingCategory_ThenPersistsCategory()
    {
        var repository = new Mock<ICategoryRepository>();

        var result = await CreateSut(repository).ExecuteAsync(new CreateCategoryRequest { Name = "Work", Color = "#FFFFFF" });

        Assert.True(result.IsSuccess);
        Assert.Equal("Work", result.Value.Name);
        repository.Verify(item => item.AddAsync(It.Is<Category>(category => category.Name == "Work"), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateCategoryUseCase CreateSut(Mock<ICategoryRepository> repository)
    {
        var validator = new Mock<IValidator<CreateCategoryRequest>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<CreateCategoryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        return new CreateCategoryUseCase(repository.Object, validator.Object);
    }

    private static Mock<IValidator<T>> CreateValidator<T>(bool isValid) where T : class
    {
        var validator = new Mock<IValidator<T>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(isValid ? new ValidationResult() : new ValidationResult(new[] { new ValidationFailure("Request", "Invalid") }));
        return validator;
    }
}