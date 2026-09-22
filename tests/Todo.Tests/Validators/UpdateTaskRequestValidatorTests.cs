using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;
using ToDoApp.Domain;
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
        var result = new UpdateTaskRequestValidator().Validate(new UpdateTaskRequest { Title = Optional<string>.Of("ab") });

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Aceita atualização válida")]
    public void GivenValidUpdate_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new UpdateTaskRequestValidator().Validate(new UpdateTaskRequest
        {
            Title = Optional<string>.Of("Updated title"),
            Priority = Optional<Priority?>.Of(Priority.Medium),
            DueDate = Optional<DateTime?>.Of(DateTime.UtcNow.AddMinutes(1))
        });

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Aceita campo explicitamente nulo em patch")]
    public void GivenExplicitNullField_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new UpdateTaskRequestValidator().Validate(new UpdateTaskRequest
        {
            Title = Optional<string>.Of(null),
            Description = Optional<string?>.Of(null),
            Priority = Optional<Priority?>.Of(null),
            DueDate = Optional<DateTime?>.Of(null),
            CategoryId = Optional<Guid?>.Of(null)
        });

        Assert.True(result.IsValid);
    }
}
