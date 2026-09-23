using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Tests.Validators;

public class UpdateTaskRequestValidatorTests
{
    [Fact(DisplayName = "Aceita atualização vazia para preservar valores atuais")]
    public void GivenEmptyUpdate_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new UpdateTaskRequestValidator().Validate(new UpdateTaskRequest());

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita título de atualização inválido")]
    public void GivenInvalidTitle_WhenValidatingRequest_ThenReturnsValidationError()
    {
        var result = new UpdateTaskRequestValidator().Validate(new UpdateTaskRequest { Title = "ab" });

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Aceita atualização válida")]
    public void GivenValidUpdate_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new UpdateTaskRequestValidator().Validate(new UpdateTaskRequest
        {
            Title = "Updated title",
            Priority = Priority.Medium,
            DueDate = DateTime.UtcNow.AddMinutes(1)
        });

        Assert.True(result.IsValid);
    }
}