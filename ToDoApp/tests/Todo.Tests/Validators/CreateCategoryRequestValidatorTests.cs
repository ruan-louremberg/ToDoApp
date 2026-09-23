using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests.Validators;

public class CreateCategoryRequestValidatorTests
{
    [Fact(DisplayName = "Rejeita nome de categoria inválido")]
    public void GivenInvalidName_WhenValidatingRequest_ThenReturnsValidationError()
    {
        var result = new CreateCategoryRequestValidator().Validate(new CreateCategoryRequest
        {
            Name = "A",
            Color = "#FFFFFF"
        });

        Assert.False(result.IsValid);
    }

    [Theory(DisplayName = "Rejeita cor fora do formato hexadecimal")]
    [InlineData("")]
    [InlineData("FFFFFF")]
    [InlineData("#FFF")]
    public void GivenInvalidColor_WhenValidatingRequest_ThenReturnsValidationError(string color)
    {
        var result = new CreateCategoryRequestValidator().Validate(new CreateCategoryRequest
        {
            Name = "Work",
            Color = color
        });

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Aceita categoria com nome e cor válidos")]
    public void GivenValidRequest_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new CreateCategoryRequestValidator().Validate(new CreateCategoryRequest
        {
            Name = "Work",
            Color = "#12aBc9"
        });

        Assert.True(result.IsValid);
    }
}