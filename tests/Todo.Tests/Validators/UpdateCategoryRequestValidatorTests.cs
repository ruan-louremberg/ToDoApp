using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests.Validators;

public class UpdateCategoryRequestValidatorTests
{
    [Fact(DisplayName = "Aceita categoria com nome e cor válidos")]
    public void GivenValidCategoryUpdate_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new UpdateCategoryRequestValidator().Validate(new UpdateCategoryRequest
        {
            Name = "Casa",
            Color = "#FFAA00"
        });

        Assert.True(result.IsValid);
    }

    [Theory(DisplayName = "Rejeita categoria com nome ou cor vazios")]
    [InlineData("", "#FFAA00")]
    [InlineData("Casa", "")]
    [InlineData(null, "#FFAA00")]
    [InlineData("Casa", null)]
    public void GivenMissingRequiredFields_WhenValidatingRequest_ThenReturnsFailure(string? name, string? color)
    {
        var result = new UpdateCategoryRequestValidator().Validate(new UpdateCategoryRequest
        {
            Name = name ?? string.Empty,
            Color = color ?? string.Empty
        });

        Assert.False(result.IsValid);
    }
}
