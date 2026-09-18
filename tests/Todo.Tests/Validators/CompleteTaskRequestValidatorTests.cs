using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Tests.Validators;

public class CompleteTaskRequestValidatorTests
{
    [Theory(DisplayName = "Aceita status válido para atualização da tarefa")]
    [InlineData(Status.Pending)]
    [InlineData(Status.InProgress)]
    [InlineData(Status.Completed)]
    public void GivenValidStatus_WhenValidatingRequest_ThenReturnsSuccess(Status status)
    {
        var result = new CompleteTaskRequestValidator().Validate(new CompleteTaskRequest { Status = status });

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita status inválido")]
    public void GivenInvalidStatus_WhenValidatingRequest_ThenReturnsValidationError()
    {
        var result = new CompleteTaskRequestValidator().Validate(new CompleteTaskRequest { Status = (Status)99 });

        Assert.False(result.IsValid);
    }
}