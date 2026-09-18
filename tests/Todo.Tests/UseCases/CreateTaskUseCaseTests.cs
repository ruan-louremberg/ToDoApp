using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class CreateTaskUseCaseTests
{
    [Fact(DisplayName = "Retorna erro quando a requisição é inválida")]
    public async Task GivenInvalidRequest_WhenCreatingTask_ThenReturnsBadRequest()
    {
        var repository = new Mock<IToDoRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();
        var validator = CreateValidator<CreateTaskRequest>(false);

        var result = await new CreateTaskUseCase(repository.Object, categoryRepository.Object, validator.Object)
            .ExecuteAsync(new CreateTaskRequest());

        Assert.True(result.IsFailed);
        repository.Verify(item => item.AddAsync(It.IsAny<ToDo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Retorna não encontrado quando a categoria não existe")]
    public async Task GivenMissingCategory_WhenCreatingTask_ThenReturnsNotFound()
    {
        var categoryRepository = new Mock<ICategoryRepository>();
        categoryRepository.Setup(item => item.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await CreateSut(categoryRepository: categoryRepository).ExecuteAsync(
            new CreateTaskRequest { Title = "Task", CategoryId = Guid.NewGuid() });

        Assert.True(result.IsFailed);
        Assert.Equal(404, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Cria tarefa e associa categoria existente")]
    public async Task GivenValidRequestAndExistingCategory_WhenCreatingTask_ThenPersistsTaskAndReturnsResponse()
    {
        var category = new Category("Work", "#FFFFFF");
        var categoryRepository = new Mock<ICategoryRepository>();
        categoryRepository.Setup(item => item.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        var repository = new Mock<IToDoRepository>();

        var result = await CreateSut(repository, categoryRepository).ExecuteAsync(new CreateTaskRequest
        {
            Title = "Task",
            Priority = Priority.High,
            CategoryId = category.Id
        });

        Assert.True(result.IsSuccess);
        Assert.Equal("Task", result.Value.Title);
        Assert.Equal(category.Id, result.Value.Category!.Id);
        repository.Verify(item => item.AddAsync(It.Is<ToDo>(task => task.CategoryId == category.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateTaskUseCase CreateSut(
        Mock<IToDoRepository>? repository = null,
        Mock<ICategoryRepository>? categoryRepository = null)
    {
        return new CreateTaskUseCase(
            (repository ?? new Mock<IToDoRepository>()).Object,
            (categoryRepository ?? new Mock<ICategoryRepository>()).Object,
            CreateValidator<CreateTaskRequest>().Object);
    }

    private static Mock<IValidator<T>> CreateValidator<T>(bool isValid = true) where T : class
    {
        var validator = new Mock<IValidator<T>>();
        validator.Setup(item => item.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(isValid ? new ValidationResult() : new ValidationResult(new[] { new ValidationFailure("Request", "Invalid") }));
        return validator;
    }
}