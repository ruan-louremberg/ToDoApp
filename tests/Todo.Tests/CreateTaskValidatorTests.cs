using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests;

public class CreateTaskValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("AB")]
    public void Validate_ShouldFail_WhenTitleIsInvalid(string title)
    {
        var validator = new CreateTaskRequestValidator();

        var result = validator.Validate(new CreateTaskRequest
        {
            Title = title
        });

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateTaskRequest.Title));
    }

    [Fact]
    public void Validate_ShouldPass_WhenTitleIsValid()
    {
        var validator = new CreateTaskRequestValidator();

        var result = validator.Validate(new CreateTaskRequest
        {
            Title = "Título válido"
        });

        Assert.True(result.IsValid);
    }
}