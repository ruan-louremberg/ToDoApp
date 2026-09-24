using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Tests.Validators;

public class CreateTaskRequestValidatorTests
{
    [Theory(DisplayName = "Rejeita título vazio, com espaços ou menor que três caracteres")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData("  ab  ")]
    public void GivenInvalidTitle_WhenValidatingRequest_ThenReturnsValidationError(string title)
    {
        var result = new CreateTaskRequestValidator().Validate(new CreateTaskRequest { Title = title });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTaskRequest.Title));
    }

    [Theory(DisplayName = "Rejeita descrição preenchida apenas com espaços ou com menos de 10 caracteres úteis")]
    [InlineData("          ")]
    [InlineData("abc")]
    [InlineData("  abcdefghi  ")]
    public void GivenInvalidDescription_WhenValidatingRequest_ThenReturnsValidationError(string description)
    {
        var result = new CreateTaskRequestValidator().Validate(new CreateTaskRequest
        {
            Title = "Título válido",
            Description = description
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTaskRequest.Description));
    }

    [Fact(DisplayName = "Aceita uma tarefa válida")]
    public void GivenValidRequest_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new CreateTaskRequestValidator().Validate(new CreateTaskRequest
        {
            Title = "Buy milk",
            Description = new string('a', 1000),
            Priority = Priority.High,
            DueDate = DateTime.UtcNow.AddMinutes(1)
        });

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita prioridade inválida")]
    public void GivenInvalidPriority_WhenValidatingRequest_ThenReturnsValidationError()
    {
        var result = new CreateTaskRequestValidator().Validate(new CreateTaskRequest
        {
            Title = "Valid title",
            Priority = (Priority)99
        });

        Assert.False(result.IsValid);
    }
}